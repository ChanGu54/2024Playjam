using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class CurtainMeshDeformer : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    [Header("���̺� ����")]
    public float waveSpeed = 1f;
    public float waveAmount = 0.1f;
    public float waveFrequency = 2f;

    [Header("��������Ʈ ����")]
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

        // ���� ��������Ʈ�� ����
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

            // ��������Ʈ ũ��� ��ġ ����
            segment.transform.localPosition = Vector3.up * (segmentHeight * i - originalSize.y / 2 + segmentHeight / 2);
            segmentRenderer.size = new Vector2(originalSize.x, segmentHeight);

            // ��������Ʈ ��Ʈ�� ����ϴ� ��츦 ���� UV ����
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