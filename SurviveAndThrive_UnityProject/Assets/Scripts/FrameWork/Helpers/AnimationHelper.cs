using UnityEngine;
using System.Collections;

public delegate void AnimationCallBack();

public static class AnimationHelper {
    
    public static float PlayAnimation(Animator animator, object animationType, string animationName, int layerIndex = 0) {
        if (animationType is bool) {
            animator.SetBool(animationName, (bool)animationType);
        } else if (animationType is float) {
            animator.SetFloat(animationName, (float)animationType);
        } else if (animationType is int) {
            animator.SetInteger(animationName, (int)animationType);
        }

        AnimatorStateInfo animStateInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
        return animStateInfo.length;
    }

	public static float GetClipLenght(RuntimeAnimatorController rtAnimController, string animationName) {
		for (int i = 0; i < rtAnimController.animationClips.Length; i++) {
			if (rtAnimController.animationClips[i].name == animationName) {
				return rtAnimController.animationClips[i].length;
			}
		}

		return 0;
	}
}