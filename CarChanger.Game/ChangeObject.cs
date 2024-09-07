using UnityEngine;

namespace CarChanger.Game
{
    public class ChangeObject
    {
        private GameObject? _instanced;
        private GameObject[] _originals;
        private bool _hideOriginals;

        public ChangeObject(Transform parent, GameObject? instance, GameObject[] originals, bool hideOriginals, MaterialHolder holder)
        {
            _instanced = Helpers.InstantiateIfNotNull(instance, parent);
            _originals = originals;
            _hideOriginals = hideOriginals;

            if (hideOriginals)
            {
                foreach (var item in originals)
                {
                    item.SetActive(false);
                }
            }

            ComponentProcessor.ProcessComponents(_instanced, holder);
        }

        public void Clear()
        {
            Helpers.DestroyIfNotNull(_instanced);

            if (_hideOriginals)
            {
                foreach (var item in _originals)
                {
                    if (item == null) continue;

                    item.SetActive(true);
                }
            }
        }
    }
}
