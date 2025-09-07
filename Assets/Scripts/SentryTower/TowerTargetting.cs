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
        if (EnemiesInRange.Length == 0) return null;

        NativeArray<EnemyData> EnemiesToCalculate = new NativeArray<EnemyData>(EnemiesInRange.Length, Allocator.TempJob);

        for (int i = 0; i < EnemiesInRange.Length; i++)
        {
            EnemyMovement currentEnemy = EnemiesInRange[i].GetComponentInParent<EnemyMovement>();
            if (currentEnemy == null) continue;

            EnemiesToCalculate[i] = new EnemyData(
                currentEnemy.transform.position,
                currentEnemy.NodeIndex,
                currentEnemy.Health,
                currentEnemy.GetInstanceID()
            );
        }

        NativeReference<int> enemyID = new NativeReference<int>(-1, Allocator.TempJob);
        NativeReference<float> compareValue = new NativeReference<float>(0, Allocator.TempJob);

        if (TargetMethod == TargetType.Last)
        {
            compareValue.Value = Mathf.NegativeInfinity;
        }
        else
        {
            compareValue.Value = Mathf.Infinity;
        }

        SearchForEnemy EnemySearchJob = new SearchForEnemy
        {
            _EnemiesToCalculate = EnemiesToCalculate,
            _NodePositions = new NativeArray<Vector3>(GameLoop.NodePositions, Allocator.TempJob),
            _NodeDistances = new NativeArray<float>(GameLoop.NodeDistance, Allocator.TempJob),
            TowerPosition = CurrentTower.transform.position,
            _EnemyToIndex = enemyID,
            _CompareValue = compareValue,
            TargetingType = (int)TargetMethod
        };

        JobHandle SearchJobHandle = EnemySearchJob.Schedule(EnemiesToCalculate.Length, 1);
        SearchJobHandle.Complete();

        int resultID = enemyID.Value;
        EnemyMovement finalTarget = null;
        if (resultID != -1)
        {
            foreach (var enemy in EntitySummoner.EnemiesInGame)
            {
                if (enemy != null && enemy.GetInstanceID() == resultID)
                {
                    finalTarget = enemy;
                    break;
                }
            }
        }

        EnemiesToCalculate.Dispose();
        EnemySearchJob._NodePositions.Dispose();
        EnemySearchJob._NodeDistances.Dispose();
        enemyID.Dispose();
        compareValue.Dispose();

        return finalTarget;
    }

    struct EnemyData
    {
        public EnemyData(Vector3 position, int waypointIndex, float hp, int instanceID)
        {
            EnemyPosition = position;
            WaypointIndex = waypointIndex;
            Health = hp;
            InstanceID = instanceID;
        }
        public Vector3 EnemyPosition;
        public int WaypointIndex;
        public float Health;
        public int InstanceID;
    }

    struct SearchForEnemy : IJobParallelFor
    {
        [ReadOnly] public NativeArray<EnemyData> _EnemiesToCalculate;
        [ReadOnly] public NativeArray<Vector3> _NodePositions;
        [ReadOnly] public NativeArray<float> _NodeDistances;
        [ReadOnly] public Vector3 TowerPosition;

        public NativeReference<int> _EnemyToIndex;
        public NativeReference<float> _CompareValue;

        [ReadOnly] public int TargetingType;

        public void Execute(int index)
        {
            float currentValue = 0;
            switch (TargetingType)
            {
                case 0: // First
                    currentValue = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (currentValue < _CompareValue.Value)
                    {
                        _EnemyToIndex.Value = _EnemiesToCalculate[index].InstanceID;
                        _CompareValue.Value = currentValue;
                    }
                    break;
                case 1: // Last
                    currentValue = GetDistanceToEnd(_EnemiesToCalculate[index]);
                    if (currentValue > _CompareValue.Value)
                    {
                        _EnemyToIndex.Value = _EnemiesToCalculate[index].InstanceID;
                        _CompareValue.Value = currentValue;
                    }
                    break;
                case 2: // Close
                    currentValue = Vector3.Distance(TowerPosition, _EnemiesToCalculate[index].EnemyPosition);
                    if (currentValue < _CompareValue.Value)
                    {
                        _EnemyToIndex.Value = _EnemiesToCalculate[index].InstanceID;
                        _CompareValue.Value = currentValue;
                    }
                    break;
            }
        }

        private float GetDistanceToEnd(EnemyData EnemyToEvaluate)
        {
            float FinalDistance = Vector3.Distance(EnemyToEvaluate.EnemyPosition, _NodePositions[EnemyToEvaluate.WaypointIndex]);
            for (int i = EnemyToEvaluate.WaypointIndex; i < _NodeDistances.Length; i++)
            {
                FinalDistance += _NodeDistances[i];
            }
            return FinalDistance;
        }
    }
}