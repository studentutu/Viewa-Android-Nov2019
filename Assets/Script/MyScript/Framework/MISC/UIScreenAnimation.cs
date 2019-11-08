using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OTPL.UI;

public class UIScreenAnimation : MonoBehaviour {

	public enum OnHideAnimation {
		None,
		Disable,
		Destory,
	}
	public bool showOnAwake = true;
	private Animator m_Animator;

	public OnHideAnimation onHideAction = OnHideAnimation.Disable;

	public Animator animator {
		get {
			if (this.m_Animator == null) {
				this.m_Animator = gameObject.GetComponent<Animator> ();
			}
			return this.m_Animator;
		}
	}
	public bool isPlaying {
		get {
			AnimatorStateInfo currentState = this.animator.GetCurrentAnimatorStateInfo(0);
			return (currentState.IsName("Show") || currentState.IsName("Hide"))
				&& currentState.normalizedTime < 1;
		}
	}
	public bool isShow {
		get {
			if (this.animator.runtimeAnimatorController == null) {
				return this.gameObject.activeSelf;
			}
			if (!this.animator.isInitialized) {
				return false;
			}
			return this.animator.GetBool("Is Show");
		}
		protected set {
			

//			this.animator.SetTrigger("Play");
			if (AppManager.Instnace.ScreenAnimationTransition == eScreenAnimationTransition.BackScreenTransition) {
				this.animator.SetBool ("IsBack", true);
			} else if (AppManager.Instnace.ScreenAnimationTransition == eScreenAnimationTransition.SelectScreenTransition) {
				this.animator.SetBool ("IsBack",false);
			}
			this.animator.SetBool("Is Show", value);
			if (!value) {
				this.OnHide();
			}
		}
	}

	protected virtual void OnHide() {
		if (!this.isShow) {
			switch (this.onHideAction) {
			case UIScreenAnimation.OnHideAnimation.None:
				break;
			case UIScreenAnimation.OnHideAnimation.Disable:
				this.gameObject.SetActive(false);
				break;
			case UIScreenAnimation.OnHideAnimation.Destory:
				Destroy(this.gameObject);
				break;
			}
		}
	}
}
