using UnityEngine;
using UnityEngine.UI;
public class GuardBar : MonoBehaviour
{
    private const float GUARDBAR_WIDTH = 150.32f;
    public const float G_REGAINTIMER_MAX = 2f;
    [SerializeField] private Slider guardFill;
    [SerializeField] private Transform g_dmgTemplate;
    [SerializeField] private GameObject guardBar_shineEffect;
    [HideInInspector] public float regainGuardTimer = G_REGAINTIMER_MAX;
    [SerializeField] private tomatoControl tomatocontrol;

    private void Update()
    {
        
        if(!tomatoControl.isGuard)
        {
            regainGuardTimer -= Time.deltaTime;
            if(regainGuardTimer < 0 && tomatocontrol.current_guardPt < tomatocontrol.maxGuard)
            {
                float regainSpeed = 2f;
                guardFill.value += regainSpeed * Time.deltaTime;
                tomatocontrol.current_guardPt = guardFill.value;
            }
        }

        if(Enemy_parried.pjParried)
        {
            RestoreGuardBar();
        }
    }

    public void SetMaxGuard(float guardPt)
    {
        guardFill.maxValue = guardPt;
    }
    public void SetGuardbar(float guardPt)
    {
        guardFill.value = guardPt;
    }

    public void RestoreGuardBar()
    {
        guardFill.value = tomatocontrol.current_guardPt = tomatocontrol.maxGuard;
        Instantiate(guardBar_shineEffect);
    }

    public void guardDamaged(float difference_guardPt)
    { 
        Transform g_dmgBar = Instantiate(g_dmgTemplate, transform);
        g_dmgBar.gameObject.SetActive(true);
        g_dmgBar.GetComponent<RectTransform>().anchoredPosition = new Vector2( -1 * guardFill.normalizedValue * GUARDBAR_WIDTH - 75.16003f, g_dmgBar.GetComponent<RectTransform>().anchoredPosition.y);
        g_dmgBar.GetComponent<Image>().fillAmount = difference_guardPt;
        g_dmgBar.gameObject.AddComponent<GuardBar_FallDown>();
    }

}
