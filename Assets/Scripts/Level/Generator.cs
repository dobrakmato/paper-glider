using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    
    // used for static and moving parts
    public class SimplePartGenerator : MonoBehaviour
    {
        public List<GameObject> Parts = new List<GameObject>();

        /// <summary>
        /// Generates random StaticObstaclePart at specified position and returns the length of generated structure.
        /// </summary>
        /// <param name="positionAt">position to generate structure at</param>
        /// <param name="difficulty">required difficulty of the generated part</param>
        /// <returns>generated structure</returns>
        public GameObject Generate(Vector3 positionAt, float difficulty)
        {
            var idx = Random.Range(0, Parts.Count); // todo choose by difficulty
            var template = Parts[idx];
            return Instantiate(template, positionAt, Quaternion.identity);
        }
    }

    public class BrakingAirplanePart : MonoBehaviour
    {
        public GameObject Generate(Vector3 positionAt, float difficulty)
        {
            return null;
        }
    }

    public enum SectionType
    {
        
    }

    public abstract class Section
    {
        public float LocalDifficulty = 1f;
        public int Parts; // number of generated parts

        public void Generate(Vector3 positionAt, float difficulty, int parts)
        {
            GenerateParts();
            GenerateCoins();
        }

        protected abstract void GenerateCoins();

        protected abstract void GenerateParts();
    }

    public class Level
    {
        private const float Length = 5000f;

        private static readonly List<Section> Sections = new List<Section>
        {
        };

        private int _seed; // unix timestamp
        private float _position; // in z-coordinate

        public bool IsEndOfLevel
        {
            get { return _position >= Length; }
        }

        public void Initialize()
        {
            _seed = Seeder.ComputeSeed();
            Random.InitState(_seed);
        }

        public void GenerateNextSection()
        {
        }
    }
}