using CarChanger.Common;
using CarChanger.Common.Configs;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using System.Collections.Generic;
using UnityEngine;

namespace CarChanger.Game
{
    internal class BufferChanger
    {
        private static Mesh? s_buffer02;
        private static Mesh? s_buffer03;
        private static Mesh? s_buffer04;
        private static Mesh? s_buffer05;
        private static Mesh? s_buffer06;
        private static Mesh? s_buffer07;
        private static Mesh? s_buffer08;
        private static Mesh? s_buffer09;
        private static Mesh? s_s282;

        private List<MeshRenderer> _renderers;
        private List<MeshFilter> _meshes;
        private Material? _ogMaterial;
        private Mesh? _ogMesh;

        public BufferChanger(CarConfig config, TrainCar car)
        {
            _renderers = new List<MeshRenderer>();
            _meshes = new List<MeshFilter>();

            if (config.BufferType == BufferType.Custom && config.CustomBuffer == null) return;

            var buffers = car.GetComponentsInChildren<TrainBufferController>();

            foreach (var item in buffers)
            {
                _renderers.Add(item.bufferModelRight.GetComponentInChildren<MeshRenderer>());
                _renderers.Add(item.bufferModelLeft.GetComponentInChildren<MeshRenderer>());
                _meshes.Add(item.bufferModelRight.GetComponentInChildren<MeshFilter>());
                _meshes.Add(item.bufferModelLeft.GetComponentInChildren<MeshFilter>());
            }

            var mesh = config.BufferType == BufferType.Custom ? config.CustomBuffer : GetBufferMesh(config.BufferType);

            foreach (var item in _meshes)
            {
                if (_ogMesh == null)
                {
                    _ogMesh = item.sharedMesh;
                }

                item.sharedMesh = mesh;
            }

            var mat = config.BufferType == BufferType.Custom ? config.CustomBufferMaterial : null;

            if (mat != null)
            {
                foreach (var item in _renderers)
                {
                    if (_ogMaterial == null)
                    {
                        _ogMaterial = item.sharedMaterial;
                    }

                    item.sharedMaterial = mat;
                }
            }
        }

        public void Reset()
        {
            if (_ogMesh != null)
            {
                foreach (var item in _meshes)
                {
                    item.sharedMesh = _ogMesh;
                }
            }

            if (_ogMaterial != null)
            {
                foreach (var item in _renderers)
                {
                    item.sharedMaterial = _ogMaterial;
                }
            }
        }

        private static Mesh GetBufferMesh(BufferType bufferType) => bufferType switch
        {
            BufferType.Buffer02 => Helpers.GetCached(ref s_buffer02, () => GetMesh(bufferType)),
            BufferType.Buffer03 => Helpers.GetCached(ref s_buffer03, () => GetMesh(bufferType)),
            BufferType.Buffer04 => Helpers.GetCached(ref s_buffer04, () => GetMesh(bufferType)),
            BufferType.Buffer05 => Helpers.GetCached(ref s_buffer05, () => GetMesh(bufferType)),
            BufferType.Buffer06 => Helpers.GetCached(ref s_buffer06, () => GetMesh(bufferType)),
            BufferType.Buffer07 => Helpers.GetCached(ref s_buffer07, () => GetMesh(bufferType)),
            BufferType.Buffer08 => Helpers.GetCached(ref s_buffer08, () => GetMesh(bufferType)),
            BufferType.Buffer09 => Helpers.GetCached(ref s_buffer09, () => GetMesh(bufferType)),
            BufferType.S282 => Helpers.GetCached(ref s_s282, () => GetMesh(bufferType)),
            _ => throw new System.ArgumentException(nameof(bufferType)),
        };

        private static Mesh GetMesh(BufferType type) =>
            ((TrainCarType)type).ToV2().prefab.GetComponentInChildren<TrainBufferController>().bufferModelLeft.GetComponent<MeshFilter>().sharedMesh;
    }
}
