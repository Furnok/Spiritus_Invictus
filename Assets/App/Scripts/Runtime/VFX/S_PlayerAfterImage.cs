using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class S_PlayerAfterImage : MonoBehaviour
{
    [TabGroup("Settings")]
    [Title("General")]
    [SuffixLabel("s", Overlay = true)]
    [SerializeField] private float _ghostLifetime = 0.3f;

    [TabGroup("Settings")]
    [SerializeField] private int _ghostCount = 6;

    [TabGroup("References")]
    [Title("Meshs")]
    [SerializeField] private SkinnedMeshRenderer[] _skinnedMeshes;

    [TabGroup("References")]
    [Title("Material")]
    [SerializeField] private Material _ghostMaterial;

    [TabGroup("Inputs")]
    [SerializeField] private RSE_OnPlayerDodgePerfect _onPerfectDodge;

    [TabGroup("Outputs")]
    [SerializeField] private SSO_PlayerStats _playerStats;

    private Transform _root = null;

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

    private void StartAfterimageBurst()
    {
        StartCoroutine(Co_AfterimageBurst());
    }

    private IEnumerator Co_AfterimageBurst()
    {
        for (int i = 0; i < _ghostCount; i++)
        {
            SpawnOneSnapshot();
            yield return new WaitForSeconds(_playerStats.Value.dodgeDuration / _ghostCount); //or use the _spawnInterval
        }
    }

    private void SpawnOneSnapshot()
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

    private IEnumerator Co_FadeAndDestroy(GameObject go, Material mat, float lifetime)
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