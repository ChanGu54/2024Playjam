using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CurtainMeshDeformer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("웨이브 설정")]
    public float waveSpeed = 1f;
    public float waveAmount = 0.1f;
    public float waveFrequency = 2f;

    [Header("스프라이트 분할")]
    public int segments = 10;

    private GameObject[] curtainSegments;
    private Vector2 originalSize;
    private float segmentHeight;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalSize = spriteRenderer.bounds.size;
        segmentHeight = originalSize.y / segments;

        CreateCurtainSegments();

        // 원본 스프라이트는 숨김
        spriteRenderer.enabled = false;
    }

    void CreateCurtainSegments()
    {
        curtainSegments = new GameObject[segments];

        for (int i = 0; i < segments; i++)
        {
            GameObject segment = new GameObject($"CurtainSegment_{i}");
            segment.transform.parent = transform;

            SpriteRenderer segmentRenderer = segment.AddComponent<SpriteRenderer>();
            segmentRenderer.sprite = spriteRenderer.sprite;
            segmentRenderer.sortingOrder = spriteRenderer.sortingOrder;

            // 스프라이트 크기와 위치 조정
            segment.transform.localPosition = Vector3.up * (segmentHeight * i - originalSize.y / 2 + segmentHeight / 2);
            segmentRenderer.size = new Vector2(originalSize.x, segmentHeight);

            // 스프라이트 시트를 사용하는 경우를 위한 UV 조정
            Bounds spriteBounds = spriteRenderer.sprite.bounds;
            float uvHeight = segmentHeight / originalSize.y;
            float uvY = (float)i / segments;

            segmentRenderer.drawMode = SpriteDrawMode.Sliced;
            segmentRenderer.tileMode = SpriteTileMode.Continuous;

            curtainSegments[i] = segment;
        }
    }

    void Update()
    {
        for (int i = 0; i < segments; i++)
        {
            if (curtainSegments[i] != null)
            {
                float yPercent = (float)i / segments;
                float wave = Mathf.Sin(yPercent * waveFrequency + Time.time * waveSpeed);
                float xOffset = wave * waveAmount * (1 - yPercent);

                Vector3 newPosition = curtainSegments[i].transform.localPosition;
                newPosition.x = xOffset;
                curtainSegments[i].transform.localPosition = newPosition;
            }
        }
    }
}