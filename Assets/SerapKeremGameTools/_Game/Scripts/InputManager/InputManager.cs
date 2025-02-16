//using SerapKeremGameTools._Game._Singleton;
//using UnityEngine;

//namespace SerapKeremGameTools._Game._InputSystem
//{
//    public class InputManager : MonoSingleton<InputManager>
//    {
//        private PlayerInput playerInput;
//        private PlayerFPSInput fpsInput;
//        private PlayerTPSInput tpsInput;
//        private PlayerTopDownInput topDownInput;

//        // Bu örnekte aktif kontrol türünü manuel olarak de?i?tirece?iz.
//        private enum ControlType
//        {
//            FPS,
//            TPS,
//            TopDown
//        }

//        [SerializeField] private ControlType currentControlType;

//        protected override void Awake()
//        {
//            base.Awake(); 

//            // Farkl? türler için input i?leyicileri olu?turuyoruz
//            fpsInput = new PlayerFPSInput(playerInput);
//            tpsInput = new PlayerTPSInput(playerInput);
//            topDownInput = new PlayerTopDownInput(playerInput);
//        }

//        private void Update()
//        {
//            // Aktif kontrol türüne göre input i?lemi yap?yoruz
//            switch (currentControlType)
//            {
//                case ControlType.FPS:
//                    fpsInput.UpdateInput();
//                    break;
//                case ControlType.TPS:
//                    tpsInput.UpdateInput();
//                    break;
//                case ControlType.TopDown:
//                    topDownInput.UpdateInput();
//                    break;
//            }
//        }

//        private void OnDestroy()
//        {
//            // Input i?leyicilerini temizliyoruz
//            fpsInput.Dispose();
//            tpsInput.Dispose();
//            topDownInput.Dispose();
//        }
//    }
//}
