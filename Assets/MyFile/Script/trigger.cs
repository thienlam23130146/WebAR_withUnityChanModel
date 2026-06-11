using System.Xml.Serialization;
using UnityEngine;

public class trigger : MonoBehaviour
{
    Animator animator;
    private int faciallayerIndex;
    private int lastEmotionValue = -1;
    private int lastActionValue = -1;
    private float currentWeight = 0f;

    public float speed = 5f; // Speed of weight change

    [Header("control")]
    [Range(0, 1)] public float targetWeight;
    [Range(0, 12)] public int Action;
    [Range(0, 9)] public int FaceEmotion;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();

        if (animator != null) {
            faciallayerIndex = animator.GetLayerIndex("Face");
            currentWeight = animator.GetLayerWeight(faciallayerIndex);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // làm mượt trọng số
        float currentWeight = animator.GetLayerWeight(faciallayerIndex);
        if (!Mathf.Approximately(currentWeight, targetWeight))
        {
            float newWeight = Mathf.MoveTowards(currentWeight, targetWeight, speed * Time.deltaTime);
            animator.SetLayerWeight(faciallayerIndex, newWeight);
        }

        // update state
        setEmotion(FaceEmotion);
        setAction(Action);
    }

    void setEmotion(int faceID) {
        if (faceID != lastEmotionValue)
        {
            animator.SetInteger("Emotion", faceID);
            lastEmotionValue = faceID;
        }
    }
    void setAction(int actionID)
    {
        if (actionID != lastActionValue)
        {
            ExecuteAction(actionID);
            lastActionValue = actionID;
        }
    }

    void ExecuteAction(int id)
    {
        // 1. Cập nhật số ID để Animator biết phải vào hành động nào
        animator.SetInteger("ActionID", id);
        // 2. Kích hoạt Trigger để "mở cửa" cho phép chuyển trạng thái
        animator.SetTrigger("Active");
    }
}
