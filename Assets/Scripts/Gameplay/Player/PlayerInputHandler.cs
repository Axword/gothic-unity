using UnityEngine;
using UnityEngine.InputSystem;
using ZelaznaDroga.Core.Attributes;
using ZelaznaDroga.Core.Utilities;
using ZelaznaDroga.Gameplay.Combat;

namespace ZelaznaDroga.Gameplay.Player
{
    /// <summary>
    /// Bridges Input System to PlayerController + Combat + Spells.
    /// </summary>
    [RequireComponent(typeof(PlayerController))]
    public class PlayerInputHandler : BaseMonoBehaviour
    {
        private PlayerController _controller;
        private CombatSystem _combat;
        private SpellCaster _spellCaster;

        protected override void Awake()
        {
            base.Awake();
            _controller = GetComponent<PlayerController>();
            _combat = GetComponent<CombatSystem>();
            _spellCaster = GetComponent<SpellCaster>();
        }

        public void OnMove(InputAction.CallbackContext ctx) => _controller?.OnMove(ctx);
        public void OnLook(InputAction.CallbackContext ctx) => _controller?.OnLook(ctx);
        public void OnJump(InputAction.CallbackContext ctx) => _controller?.OnJump(ctx);
        public void OnSprint(InputAction.CallbackContext ctx) => _controller?.OnSprint(ctx);

        public void OnAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.started && _combat != null)
            {
                _combat.PerformLightAttack();
            }
        }

        public void OnHeavyAttack(InputAction.CallbackContext ctx)
        {
            if (ctx.started && _combat != null)
            {
                _combat.PerformHeavyAttack();
            }
        }

        public void OnInteract(InputAction.CallbackContext ctx)
        {
            if (ctx.started)
            {
                var inter = ComponentLocator.Get<ZelaznaDroga.Gameplay.Interaction.IInteractionSystem>();
                inter?.TryInteract();
            }
        }

        // Spell keys
        public void OnCastSpell(InputAction.CallbackContext ctx)
        {
            if (ctx.started && _spellCaster != null)
            {
                _spellCaster.CastActiveSpell();
            }
        }

        private void Update()
        {
            // Keyboard fallbacks for spells (F = fireball, G = ice)
            if (Input.GetKeyDown(KeyCode.F) && _spellCaster != null)
            {
                _spellCaster.CastActiveSpell();
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _combat?.SwitchToWeapon(1);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                // switch to bow or second weapon
                _combat?.SwitchToWeapon(2);
            }
        }
    }
}
