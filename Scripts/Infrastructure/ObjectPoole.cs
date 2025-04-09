using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    public class ObjectPoole<T> where T: Component
    {
        private List<T> _pool;
        private int _size;
        private T _prefab;
        private Transform _parent;

        public void CreatePool(T prefab, int size, Transform parent)
        {
            _size = size;
            _pool = new List<T>(size);

            _prefab = prefab;
            _parent = parent;
            for (int i = 0; i < size; i++)
            {
                var obj = Object.Instantiate(_prefab, _parent, true);
                obj.gameObject.SetActive(false);
                _pool.Add(obj);
            }
        }
        public void CreatePool(List<T> prefab, int size, Transform parent)
        {
            
            _size = size;
            _pool = new List<T>(size);

            for (int i = 0; i < size; i++)
            {
                int rnd = Random.Range(0, prefab.Count);
                var obj = Object.Instantiate(prefab[rnd], parent, true);
                obj.gameObject.SetActive(false);
                _pool.Add(obj);
            }
        }

        public T GetObject()
        {
            var isGet=false;
            foreach (T obj in _pool)
            {
                if (!obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.SetActive(true);
                    isGet = true;
                    return obj;
                }
            }

            if (!isGet)
            {
                var obj = Object.Instantiate(_prefab, _parent, true);
                _pool.Add(obj);
                return obj;
            }
            return null;
        }

        public void HideAll()
        {
            foreach (T obj in _pool)
            {
                if (obj.gameObject.activeInHierarchy)
                {
                    obj.gameObject.transform.position = _parent.position;
                    obj.gameObject.SetActive(false);
                }
            }
        }
    }
}