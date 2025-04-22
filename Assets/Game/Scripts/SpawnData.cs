using UnityEngine;

public class SpawnData : MonoBehaviour
{
    [SerializeField] private GameObject _buisnessPrefab;
    [SerializeField] private Transform _buisnessTransform;
    [SerializeField] private GameObject _balancePrefab;
    [SerializeField] private Transform _balanceTransform;

    public GameObject BuisnessPrefab => _buisnessPrefab;
    public Transform BuisnessTransform => _buisnessTransform;
    public GameObject BalancePrefab => _balancePrefab;
    public Transform BalanceTransform => _balanceTransform;
}