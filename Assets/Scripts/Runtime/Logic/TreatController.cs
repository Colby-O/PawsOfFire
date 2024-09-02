using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PawsOfFire.Logic
{
    [System.Serializable]
    public class SpawnPoint
    {
        public Transform transform;
        public bool isUsed = false;
        public int id;
    }

    internal class TreatController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Treat _treatPrefab;
        [SerializeField] private List<Transform> _spawnLocationsInput;
        [SerializeField] private float _treatLifeSpan;
        [SerializeField] private int _maxNumberOfTreats;
        [SerializeField, Range(0, 1)] private float _spanChange = 0.05f;

        private List<SpawnPoint> _spawnLocations;
        private List<Treat> _activeTreats;

        private bool _startSpawn = false;

        private SpawnPoint FetchSpawnPoint()
        {
            List<SpawnPoint> matching = _spawnLocations.Where(x => !x.isUsed).ToList();
            if (matching.Count == 0)
            {
                return null;
            }
            return matching[Random.Range(0, matching.Count)];
        }

        private void SpawnTreat()
        {
            SpawnPoint pt = FetchSpawnPoint();
            if (pt != null && Random.value < _spanChange) 
            {
                pt.isUsed = true;
                Treat treat = Instantiate(_treatPrefab, pt.transform);
                treat.SetLocationID(pt.id);
                _activeTreats.Add(treat);
            }
        }

        private void CheckTreatStatus()
        {
            for (int i = 0; i < _activeTreats.Count; i++)
            {
                Treat treat = _activeTreats[i];
                if (treat == null) continue;
                if (treat.GetLifespan() > _treatLifeSpan || treat.IsDestoryed())
                {
                    _spawnLocations[_activeTreats[i].GetLocationID()].isUsed = false;
                    treat.Remove();
                    _activeTreats[i] = null;
                }
            }

            for (int i = _activeTreats.Count - 1; i >= 0; i--)
            {
                if (_activeTreats[i] == null)
                {
                    _activeTreats.RemoveAt(i);
                }
            }
        }

        public void StartSpawn()
        {
            _startSpawn = true;
            _activeTreats = new List<Treat>();
            _spawnLocations = new List<SpawnPoint>();

            foreach (Transform trans in _spawnLocationsInput)
            {
                SpawnPoint pt = new SpawnPoint();
                pt.id = _spawnLocations.Count;
                pt.transform = trans;
                pt.isUsed = false;
                _spawnLocations.Add(pt);
            }
        }

        public void EndSpawn()
        {
            _startSpawn = false;
            foreach (Treat treat in _activeTreats) Destroy(treat.gameObject);
            _activeTreats = new List<Treat>();
            _spawnLocations = new List<SpawnPoint>();
        }

        private void Update()
        {
            if (!_startSpawn) return;
            if (_activeTreats.Count < _maxNumberOfTreats) SpawnTreat();
            CheckTreatStatus();
        }
    }
}
