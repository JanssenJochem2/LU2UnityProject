using DG.Tweening;
using UnityEngine;

public class ScaleAnimationHandler : MonoBehaviour
{
    public Transform ScaleAnimation;

    public void OnSelect()
    {
        ScaleUpDown();
    }

    public void Shake()
    {
        ScaleAnimation.DOShakePosition(0.2f, new Vector3(1f, 0, 0), 20, 90, false, false).SetLoops(3, LoopType.Restart);

    }

    private void ScaleUpDown()
    {
        ScaleAnimation.DOScale(new Vector3(6f, 6f, 6f), 0.5f)
                      .SetLoops(1, LoopType.Yoyo)
                      .SetEase(Ease.InOutSine)
                      .OnKill(() => ScaleAnimation.localScale = new Vector3(5f, 5f, 5f));
    }
}
