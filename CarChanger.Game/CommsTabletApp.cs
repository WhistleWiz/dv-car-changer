using CarChanger.Common;
using CarChanger.Game.Components;
using DV;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace CarChanger.Game
{
    /// <summary>
    /// Car Changer's app on the Comms Tablet (https://github.com/Specter242/DVCommsTablet).
    /// The tablet finds this class by its name and binds its members by reflection (its guest
    /// app contract, version 2: GUEST-APPS.md in that repository). Nothing here references the
    /// tablet; without it, nothing here is ever called.
    /// <para>
    /// The tablet aims and chooses the car, as for its own Paint page. This app lists the car's
    /// modifications and switches them on and off by the same rules as the comms radio's mode:
    /// one that is on goes off, one that combines with those on goes on, and one that does not
    /// asks first and, pressed again, replaces the ones it clashes with on the next frame.
    /// </para>
    /// </summary>
    public static class CommsTabletApp
    {
        public const int Contract = 2;
        public const string Id = "carchanger";
        public const string Name = "Car Changer";

        // The tablet's screen: 1080 x 924 canvas pixels, y down from the top-left corner.
        private const float Width = 1080f;
        private const int Columns = 3;
        private const int Rows = 5;
        private const int PerPage = Columns * Rows;
        private const int PreviousSlot = PerPage;
        private const int NextSlot = PerPage + 1;
        private const float PlateWidth = 332f;
        private const float PlateHeight = 112f;
        private const float Gap = 12f;
        private const float Top = 124f;
        private const float Reach = 100f;

        private static readonly Color s_cream = new Color(0.93f, 0.89f, 0.78f);
        private static readonly Color s_ink = new Color(0.1f, 0.08f, 0.05f);
        private static readonly Color s_iron = new Color(0.16f, 0.16f, 0.18f, 0.95f);
        private static readonly Color s_brass = new Color(0.78f, 0.6f, 0.25f);
        private static readonly Color s_clash = new Color(0.62f, 0.22f, 0.16f);

        private class Plate
        {
            public Image Edge = null!;
            public Image Back = null!;
            public Text Label = null!;
        }

        private class View
        {
            public RectTransform Region = null!;
            public GameObject Tablet = null!;
            public Action Invalidate = null!;
            public Action<int, Rect, bool> PlaceSlot = null!;
            public Action<int> HideSlotsFrom = null!;
            public Text Header = null!;
            public Text Footer = null!;
            public Text Empty = null!;
            public List<Plate> Plates = new List<Plate>();
            public TrainCar? Car;
            public List<ModelConfig> Configs = new List<ModelConfig>();
            public int Selected = -1;
            public int Page;
            public ModelConfig? Pending;
            public string Message = string.Empty;
            public bool Adding;
            public int RedrawFrames;
        }

        private static readonly Dictionary<int, View> s_views = new Dictionary<int, View>();
        private static int s_nextView;
        private static Texture2D? s_icon;

        #region Contract

        public static Texture2D Icon()
        {
            if (s_icon == null)
            {
                s_icon = MakeIcon();
            }

            return s_icon;
        }

        public static bool Available()
        {
            // Car Changer's changes are not shared over Multiplayer: on a client they would be
            // that player's alone, and lost. Only offered where there are modifications at all.
            return ChangeManager.LoadedConfigs.Count > 0 && !MultiplayerSession.IsClient();
        }

        public static int Open(RectTransform region, GameObject tablet, Action invalidate,
            Action<int, Rect, bool> placeSlot, Action<int> hideSlotsFrom, int slotCount, bool vr)
        {
            var view = new View
            {
                Region = region,
                Tablet = tablet,
                Invalidate = invalidate,
                PlaceSlot = placeSlot,
                HideSlotsFrom = hideSlotsFrom,
            };

            view.Header = MakeText(region, "Header", new Rect(40, 24, Width - 80, 88), 40, TextAnchor.MiddleLeft);
            view.Empty = MakeText(region, "Empty", new Rect(80, 300, Width - 160, 240), 40, TextAnchor.MiddleCenter);
            view.Footer = MakeText(region, "Footer", new Rect(40, 856, Width - 80, 56), 30, TextAnchor.MiddleLeft);

            // One plate for each place on a page, and two to turn pages.
            for (int i = 0; i < PerPage + 2; i++)
            {
                var plate = new Plate();
                var area = PlateArea(i);
                plate.Edge = MakeGraphic<Image>(region, "Edge", new Rect(area.x - 4, area.y - 4, area.width + 8, area.height + 8));
                plate.Edge.color = s_cream;
                plate.Back = MakeGraphic<Image>(region, "Plate", area);
                plate.Label = MakeText(region, "Label", new Rect(area.x + 10, area.y + 6, area.width - 20, area.height - 12), 30, TextAnchor.MiddleCenter);
                plate.Label.resizeTextForBestFit = true;
                plate.Label.resizeTextMinSize = 22;
                plate.Label.resizeTextMaxSize = 30;
                view.Plates.Add(plate);
            }

            int id = ++s_nextView;
            s_views[id] = view;
            return id;
        }

        public static void Close(int view)
        {
            s_views.Remove(view);
        }

        public static void Shown(int view, bool shown) { }

        public static void Held(int view, bool held) { }

        public static void Update(int view)
        {
            // Car Changer applies and removes changes over the next frames: show them as they land.
            if (s_views.TryGetValue(view, out var v) && v.RedrawFrames > 0)
            {
                v.RedrawFrames--;
                v.Invalidate();
            }
        }

        public static void Draw(int view)
        {
            if (!s_views.TryGetValue(view, out var v))
            {
                return;
            }

            var car = v.Car;
            bool hasCar = car != null && v.Configs.Count > 0;
            v.Header.text = car != null ? $"{Localization.GetLocalizedName(car.carLivery)} ({car.ID})" : Name;
            v.Empty.text = hasCar ? string.Empty : Localization.RadioBegin;
            v.Footer.text = v.Pending != null ? Localization.IncompatibleModification : v.Message;

            var on = hasCar ? OnCar(car!) : new List<ModelConfig>();
            int pages = Mathf.Max(1, (v.Configs.Count + PerPage - 1) / PerPage);
            v.Page = Mathf.Clamp(v.Page, 0, pages - 1);

            for (int i = 0; i < PerPage; i++)
            {
                int index = v.Page * PerPage + i;
                bool shownHere = hasCar && index < v.Configs.Count;
                var plate = v.Plates[i];
                SetActive(plate, shownHere);
                v.PlaceSlot(i, PlateArea(i), shownHere);

                if (!shownHere)
                {
                    continue;
                }

                var config = v.Configs[index];
                bool isOn = on.Contains(config);
                bool pending = v.Pending == config;
                plate.Back.color = pending ? s_clash : isOn ? s_brass : s_iron;
                plate.Edge.enabled = index == v.Selected;
                plate.Label.color = isOn && !pending ? s_ink : s_cream;
                plate.Label.text = $"{Localization.GetLocalizedName(config)}\n{(pending ? Localization.Override : isOn ? Localization.Disable : Localization.Enable)}";
            }

            // Turning pages, only when there is more than one.
            bool paged = hasCar && pages > 1;
            for (int slot = PreviousSlot; slot <= NextSlot; slot++)
            {
                var plate = v.Plates[slot];
                SetActive(plate, paged);
                v.PlaceSlot(slot, PlateArea(slot), paged);

                if (paged)
                {
                    plate.Back.color = s_iron;
                    plate.Edge.enabled = false;
                    plate.Label.color = s_cream;
                    plate.Label.text = slot == PreviousSlot ? $"<  {v.Page + 1} / {pages}" : $"{v.Page + 1} / {pages}  >";
                }
            }

            v.HideSlotsFrom(NextSlot + 1);
        }

        public static void Pressed(int view, int slot)
        {
            if (!s_views.TryGetValue(view, out var v) || v.Car == null)
            {
                return;
            }

            int pages = Mathf.Max(1, (v.Configs.Count + PerPage - 1) / PerPage);

            switch (slot)
            {
                case PreviousSlot:
                    v.Page = Helpers.Wrap(v.Page - 1, pages);
                    return;
                case NextSlot:
                    v.Page = Helpers.Wrap(v.Page + 1, pages);
                    return;
                default:
                    int index = v.Page * PerPage + slot;

                    if (slot >= 0 && slot < PerPage && index < v.Configs.Count)
                    {
                        v.Selected = index;
                        Switch(v, v.Configs[index]);
                    }
                    return;
            }
        }

        public static Rect PressArea(int view) => Rect.zero;

        public static void PressAt(int view, Vector2 canvas) { }

        /// <summary>The tablet used at the chosen car again: switches the modification stepped to, as the radio does.</summary>
        public static bool Use(int view)
        {
            if (!s_views.TryGetValue(view, out var v) || v.Car == null || v.Configs.Count == 0)
            {
                return false;
            }

            if (v.Selected < 0)
            {
                // Nothing stepped to yet: show the first, which the next use switches.
                Select(v, 0);
                return true;
            }

            Switch(v, v.Configs[v.Selected]);
            return true;
        }

        /// <summary>The scroll wheel steps through the modifications, as the radio's buttons do.</summary>
        public static bool Scroll(int view, bool forward)
        {
            if (!s_views.TryGetValue(view, out var v) || v.Car == null || v.Configs.Count == 0)
            {
                return false;
            }

            Select(v, v.Selected < 0 ? (forward ? 0 : v.Configs.Count - 1) : Helpers.Wrap(v.Selected + (forward ? 1 : -1), v.Configs.Count));
            PlaySound(v, "CommsRadio_Select_01");
            return true;
        }

        public static bool Fits(TrainCar car)
        {
            return car != null && ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out var configs) && configs.Count > 0;
        }

        public static void Chosen(int view, TrainCar? car)
        {
            if (!s_views.TryGetValue(view, out var v))
            {
                return;
            }

            v.Car = car;
            v.Configs = car != null && ChangeManager.LoadedConfigs.TryGetValue(car.carLivery, out var configs)
                ? configs.OrderBy(x => Localization.GetLocalizedName(x), StringComparer.CurrentCulture).ToList()
                : new List<ModelConfig>();
            v.Selected = -1;
            v.Page = 0;
            v.Pending = null;
            v.Message = string.Empty;
            v.Adding = false;
            v.Invalidate();
        }

        public static Color BeamColor()
        {
            return Color.HSVToRGB(Time.time % 1.0f, 1.0f, 1.0f);
        }

        #endregion

        #region Changes

        private static List<ModelConfig> OnCar(TrainCar car)
        {
            return car.GetAppliedChanges().Where(x => x != null && x.Config != null).Select(x => x.Config!).ToList();
        }

        private static void Select(View v, int index)
        {
            v.Selected = index;
            v.Page = index / PerPage;

            // Stepping away answers no to an override.
            v.Pending = null;
            v.Message = string.Empty;
        }

        private static void Switch(View v, ModelConfig config)
        {
            var car = v.Car;

            if (car == null)
            {
                return;
            }

            var pending = v.Pending;
            v.Pending = null;
            v.Message = string.Empty;

            // A press on the screen needs no aim, so it keeps to the radio's reach.
            var player = PlayerManager.PlayerTransform;

            if (player == null || (car.transform.position - player.position).sqrMagnitude > Reach * Reach)
            {
                v.Message = Localization.TooFar;
                PlaySound(v, "CommsRadio_Cancel_01");
                return;
            }

            // Still putting the last one on.
            if (v.Adding)
            {
                return;
            }

            var applied = car.GetAppliedChanges();

            // Disable change.
            if (applied.TryFind(x => x.Config == config, out var change))
            {
                UnityEngine.Object.Destroy(change);
                Done(v);
                return;
            }

            // Can combine the new change with the others.
            if (ModelConfig.CanCombine(config, applied.Where(x => x.Config != null).Select(x => x.Config!)))
            {
                Apply(v, car, config);
                return;
            }

            // Cannot combine: ask first, as the radio's override.
            if (pending != config)
            {
                v.Pending = config;
                PlaySound(v, "CommsRadio_Cancel_01");
                return;
            }

            // Asked again: disable all conflicting changes, and apply the config with delay due to same frame shenanigans.
            foreach (var other in applied)
            {
                if (other.Config == null || !ModelConfig.CanCombine(other.Config, config))
                {
                    UnityEngine.Object.Destroy(other);
                }
            }

            v.Adding = true;
            car.StartCoroutine(DelayedApply(v, car, config));
        }

        private static System.Collections.IEnumerator DelayedApply(View v, TrainCar car, ModelConfig config)
        {
            // Wait for the next frame.
            yield return null;
            v.Adding = false;

            if (car != null)
            {
                Apply(v, car, config);
            }
        }

        private static void Apply(View v, TrainCar car, ModelConfig config)
        {
            var change = AppliedChange.AddChange(car, config);
            change.OnApply += Applied;
            Done(v);

            void Applied(AppliedChange c)
            {
                c.OnApply -= Applied;
                v.Invalidate();
            }
        }

        private static void Done(View v)
        {
            PlaySound(v, "CommsRadio_RerailDrop_01");
            v.RedrawFrames = 3;
            v.Invalidate();
        }

        private static void PlaySound(View v, string name)
        {
            if (v.Tablet != null && CarChangerMod.SoundCache.TryGetValue(name, out var clip))
            {
                CommsRadioController.PlayAudioFromRadio(clip, v.Tablet.transform);
            }
        }

        #endregion

        #region Drawing

        private static Rect PlateArea(int slot)
        {
            float left = (Width - Columns * PlateWidth - (Columns - 1) * Gap) / 2f;

            if (slot >= PerPage)
            {
                // The two page plates share the row under the last.
                float half = (Columns * PlateWidth + (Columns - 1) * Gap - Gap) / 2f;
                return new Rect(left + (slot - PerPage) * (half + Gap), Top + Rows * (PlateHeight + Gap), half, PlateHeight * 0.75f);
            }

            return new Rect(left + slot % Columns * (PlateWidth + Gap), Top + slot / Columns * (PlateHeight + Gap), PlateWidth, PlateHeight);
        }

        private static void SetActive(Plate plate, bool active)
        {
            plate.Edge.gameObject.SetActive(active);
            plate.Back.gameObject.SetActive(active);
            plate.Label.gameObject.SetActive(active);
        }

        private static T MakeGraphic<T>(RectTransform region, string name, Rect area) where T : Graphic
        {
            // On the region's layer, the tablet's isolated screen layer, and no raycasts: presses come through slots.
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = region.gameObject.layer;
            var rect = (RectTransform)go.transform;
            rect.SetParent(region, false);
            rect.anchorMin = rect.anchorMax = rect.pivot = new Vector2(0f, 1f);
            rect.anchoredPosition = new Vector2(area.x, -area.y);
            rect.sizeDelta = area.size;
            var graphic = go.AddComponent<T>();
            graphic.raycastTarget = false;
            return graphic;
        }

        private static Text MakeText(RectTransform region, string name, Rect area, int size, TextAnchor anchor)
        {
            var text = MakeGraphic<Text>(region, name, area);
            text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            text.fontSize = size;
            text.color = s_cream;
            text.alignment = anchor;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Truncate;
            return text;
        }

        // Two arrows, one each way, white on clear: a swap.
        private static Texture2D MakeIcon()
        {
            const int size = 128;
            var pixels = new Color32[size * size];

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    bool top = Arrow(x, size - 1 - y, 40, true);
                    bool bottom = Arrow(x, size - 1 - y, 88, false);
                    pixels[y * size + x] = top || bottom ? new Color32(255, 255, 255, 255) : new Color32(255, 255, 255, 0);
                }
            }

            var icon = new Texture2D(size, size, TextureFormat.RGBA32, false) { name = "CarChanger_TabletIcon" };
            icon.SetPixels32(pixels);
            icon.Apply();
            return icon;

            static bool Arrow(int x, int y, int row, bool right)
            {
                int along = right ? x : 127 - x;
                bool shaft = along >= 16 && along < 88 && Mathf.Abs(y - row) <= 7;
                bool head = along >= 80 && along < 112 && Mathf.Abs(y - row) <= 111 - along;
                return shaft || head;
            }
        }

        #endregion

        /// <summary>
        /// Whether this game is a Multiplayer client, read from the Multiplayer mod by reflection
        /// (its NetworkLifecycle); false without it.
        /// </summary>
        private static class MultiplayerSession
        {
            private static readonly Type? s_lifecycle = AccessTools.TypeByName("Multiplayer.Components.Networking.NetworkLifecycle");
            private static readonly PropertyInfo? s_client = s_lifecycle == null ? null : AccessTools.Property(s_lifecycle, "IsClientRunning");
            private static readonly PropertyInfo? s_server = s_lifecycle == null ? null : AccessTools.Property(s_lifecycle, "IsServerRunning");
            private static UnityEngine.Object? s_instance;
            private static float s_nextLook;

            public static bool IsClient()
            {
                if (s_client == null || s_server == null)
                {
                    return false;
                }

                if (s_instance == null && Time.unscaledTime >= s_nextLook)
                {
                    s_nextLook = Time.unscaledTime + 5f;
                    s_instance = UnityEngine.Object.FindObjectOfType(s_lifecycle!);
                }

                if (s_instance == null)
                {
                    return false;
                }

                try
                {
                    return (bool)s_client.GetValue(s_instance) && !(bool)s_server.GetValue(s_instance);
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }
    }
}
