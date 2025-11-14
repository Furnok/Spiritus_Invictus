using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

public class S_PlayerAfterImage : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _ghostLifetime = 0.3f;
    [SerializeField] private float _spawnInterval = 0.04f;
    [SerializeField] private int _ghostCount = 6;

    [Header("References")]
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshes;
    [SerializeField] private Material _ghostMaterial;
    [SerializeField] SSO_PlayerStats _playerStats;

    [Header("Inputs")]
    [SerializeField] RSE_OnPlayerDodgePerfect _onPerfectDodge;

    //[Header("Outputs")]

    Transform _root;

    private void Awake()
    {
        if (_root == null)
        {
            GameObject rootGO = new GameObject("GhostsRoot_Global");
            _root = rootGO.transform;
        }
    }

    private void OnEnable()
    {
        _onPerfectDodge.action += StartAfterimageBurst;
    }

    private void OnDisable()
    {
        _onPerfectDodge.action -= StartAfterimageBurst;
    }

    void StartAfterimageBurst()
    {
        StartCoroutine(Co_AfterimageBurst());
    }

    IEnumerator Co_AfterimageBurst()
    {
        for (int i = 0; i < _ghostCount; i++)
        {
            SpawnOneSnapshot();
            yield return new WaitForSeconds(_playerStats.Value.dodgeDuration / _ghostCount); //or use the _spawnInterval
        }
    }

    void SpawnOneSnapshot()
    {
        if (_skinnedMeshes == null || _skinnedMeshes.Length == 0 || _ghostMaterial == null)
            return;

        foreach (var smr in _skinnedMeshes)
        {
            if (smr == null || !smr.gameObject.activeInHierarchy)
                continue;

            var go = new GameObject("AfterimageGhost");
            go.layer = gameObject.layer;
            go.transform.SetPositionAndRotation(smr.transform.position, smr.transform.rotation);
            go.transform.localScale = smr.transform.lossyScale;
            go.transform.SetParent(_root, worldPositionStays: true);

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();

            var bakedMesh = new Mesh();
            smr.BakeMesh(bakedMesh);
            mf.sharedMesh = bakedMesh;

            var matInstance = new Material(_ghostMaterial);
            mr.sharedMaterial = matInstance;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            StartCoroutine(Co_FadeAndDestroy(go, matInstance, _ghostLifetime));
        }
    }

    IEnumerator Co_FadeAndDestroy(GameObject go, Material mat, float lifetime)
    {
        float t = 0f;
        Color baseColor = mat.HasProperty("_BaseColor")
            ? mat.GetColor("_BaseColor")
            : mat.color;

        float startAlpha = baseColor.a;

        while (t < lifetime)
        {
            float k = 1f - (t / lifetime);
            var c = baseColor;
            c.a = startAlpha * k;
            if (mat.HasProperty("_BaseColor"))
                mat.SetColor("_BaseColor", c);
            else
                mat.color = c;

            t += Time.deltaTime;
            yield return null;
        }

        Destroy(go);
        Destroy(mat);
    }
}