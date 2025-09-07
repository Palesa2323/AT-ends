using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTargetting
{
    public enum TargetType
    {
        First,
        Last,
        Close,
    }

    public static EnemyMovement GetTarget(TowerBehaviour CurrentTower, TargetType TargetMethod)
    {
        Collider[] EnemiesInRange = Physics.OverlapSphere(CurrentTower.transform.position, CurrentTower.Range, CurrentTower.EnemiesLayer);

        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length, Allocator.TempJob);
        NativeArray<Vector3> NodePositions = new NativeArray<Vector3>(GameLoop.NodePositions, Allocator.TempJob);
        NativeArray<float> NodeDistances = new NativeArray<float>(GameLoop.NodeDistance, Allocator.TempJob);
        NativeArray<int> EnemyToIndex = new NativeArray<int>(1, Allocator.TempJob);
        EnemyToIndex[0] = -1;

        int EnemyToIndexToReturn = -1;

        // FIX: changed `1 < EnemiesToCalculate.Length` → `i < EnemiesToCalculate.Length`
        for (int i = 0; i < EnemiesToCalculate.Length; i++)
        {
            EnemyMovement CurrentEnemy = EnemiesInRange[i].transform.parent.GetComponent<EnemyMovement>();
            int EnemyIndexInList = EntitySummoner.EnemiesInGame.FindIndex(x => x == CurrentEnemy);

            EnemiesToCalculate[i] = new EnemyData(
                CurrentEnemy.transform.position,
                CurrentEnemy.NodeIndex,
                CurrentEnemy.Health,
                EnemyIndexInList
            );
        }

        SearchForEnemy EnemySearchJob = new SearchForEnemy
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _NodePositions = NodePositions,
            _NodeDistances = NodeDistances,
            TowerPosition = CurrentTower.transform.position,
            _EnemyToIndex = EnemyToIndex,
            CompareValue = Mathf.Infinity,
            TargettingType = (int)TargetMethod
        };

        switch (TargetMethod)
        {
            case TargetType.First:
                EnemySearchJob.CompareValue = Mathf.Infinity;
                break;

            case TargetType.Last:
                EnemySearchJob.CompareValue = Mathf.NegativeInfinity;
                break;

            case TargetType.Close:
                EnemySearchJob.CompareValue = Mathf.Infinity;
                break;
        }

        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length, 1);
        SearchJobHandle.Complete();

        if (EnemyToIndex[0] != -1)
        {
            EnemyToIndexToReturn = EnemiesToCalculate[EnemyToIndex[0]].EnemyIndex;
        }

        EnemiesToCalculate.Dispose();
        NodePositions.Dispose();
        NodeDistances.Dispose();
        EnemyToIndex.Dispose();

        if (EnemyToIndexToReturn == -1)
        {
            return null;
        }
        return EntitySummoner.EnemiesInGame[EnemyToIndexToReturn];
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int nodeIndex, float hp, int enemyIndex)
        {
            EnemyPosition = position;
            NodeIndex = nodeIndex;
            EnemyIndex = enemyIndex;
            Health = hp;
        }

        public Vector3 EnemyPosition;
        public int EnemyIndex;
        public int NodeIndex;
        public float Health;
    }

    struct SearchForEnemy : IJobParallelFor
    {
        public NativeArray<EnemyData> _EnemiesToCalculate;
        public NativeArray<Vector3> _NodePositions;
        public NativeArray<float> _NodeDistances;
        public Vector3 TowerPosition;
        public NativeArray<int> _EnemyToIndex;
        public float CompareValue;
        public int TargettingType;

        public void Execute(int index)
        {
            float CurrentEnemyDistanceToEnd = 0;
            float DistanceToEnemy = 0;

            switch (TargettingType)
            {
                case 0: // First
                    CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (CurrentEnemyDistanceToEnd < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }
                    break;

                case 1: // Last
                    CurrentEnemyDistanceToEnd = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (CurrentEnemyDistanceToEnd > CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = CurrentEnemyDistanceToEnd;
                    }
                    break;

                case 2: // Close
                    DistanceToEnemy = Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (DistanceToEnemy < CompareValue)
                    {
                        _EnemyToIndex[0] = index;
                        CompareValue = DistanceToEnemy;
                    }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData EnemyToEvaluate)
        {
            float FinalDistance = Vector3.Distance(EnemyToEvaluate.EnemyPosition, _NodePositions[EnemyToEvaluate.NodeIndex]);

            for (int i = EnemyToEvaluate.NodeIndex; i < _NodeDistances.Length; i++)
            {
                FinalDistance += _NodeDistances[i];
            }
            return FinalDistance;
        }
    }
}
