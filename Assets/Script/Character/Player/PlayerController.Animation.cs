using UnityEngine;

namespace Orbit_Character
{
	public partial class PlayerController
	{
        /* Animation */
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDHeadLookX;
        private int _animIDHeadLookY;
        private int _animIDGunFire;
        private int _animIDGunReload;

        private Animator _animator;

        private bool _hasAnimator;

        private void InitialiseAnimations()
        {
            _hasAnimator = TryGetComponent(out _animator);

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDHeadLookX = Animator.StringToHash("HeadLookX");
            _animIDHeadLookY = Animator.StringToHash("HeadLookY");

            _animIDGunFire = Animator.StringToHash("Fire");
            _animIDGunReload = Animator.StringToHash("Reload");
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, FootstepAudioClips.Length);
                    PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void PlayClipAtPoint(AudioClip clip, Vector3 position, [UnityEngine.Internal.DefaultValue("1.0F")] float volume)
        {
            GameObject gameObject = new GameObject("One shot audio");
            gameObject.transform.position = position;
            PlaySound(clip);
            Object.Destroy(gameObject, clip.length * ((Time.timeScale < 0.01f) ? 0.01f : Time.timeScale));
        }
    }
}