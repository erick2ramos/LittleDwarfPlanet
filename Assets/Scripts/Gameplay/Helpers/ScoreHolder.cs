using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ldp
{
    public class ScoreHolder
    {
        public float initialDistanceToPlanet;
        public float minimalDistanceSoFar;
        public float timeLeft;

        public ScoreHolder()
        {

        }

        public ScoreHolder(float initialDistance, float maxTime)
        {
            timeLeft = maxTime;
            initialDistanceToPlanet = initialDistance;
            minimalDistanceSoFar = initialDistanceToPlanet;
        }

        public float PartialScore
        {
            get { return initialDistanceToPlanet - minimalDistanceSoFar; }
        }

        public float FinalScore(bool alive)
        {
            if (alive)
            {
                return (initialDistanceToPlanet - minimalDistanceSoFar) * timeLeft;
            }
            return PartialScore;
        }

        public void SetMaxDistanceTowardsPlanet(Transform currentPosition, Transform targetPlanet)
        {
            float currentDistance = Vector2.Distance(currentPosition.position, targetPlanet.position) -
                (targetPlanet.localScale.x / 2);
            minimalDistanceSoFar = Mathf.Min(minimalDistanceSoFar, currentDistance);
        }
    }
}