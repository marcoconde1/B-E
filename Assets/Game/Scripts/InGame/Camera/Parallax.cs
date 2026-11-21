using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// Efecto parallax para una capa (fondo, decoración, primer plano).
/// Se actualiza justo después de que Cinemachine mueve la cámara, así no hay temblor (jitter).
///
/// Factor de parallax:
///   1    = la capa se queda pegada a la cámara (como si estuviera infinitamente lejos)
///   0.5  = se mueve a la mitad de velocidad (fondo lejano)
///   0    = no hay parallax, se mueve igual que el mundo (piso / jugador)
///  -0.3  = se mueve MÁS rápido que el mundo (primer plano, más cerca de la cámara)
/// </summary>
public class Parallax : MonoBehaviour
{
    [Tooltip("Cámara a seguir. Si se deja vacío usa Camera.main (la que tiene el CinemachineBrain).")]
    [SerializeField] private Transform cameraTransform;

    [Tooltip("X: horizontal, Y: vertical. 1 = pegado a la cámara, 0 = sin parallax, negativo = primer plano.")]
    [SerializeField] private Vector2 parallaxFactor = new Vector2(0.5f, 0.5f);

    [Tooltip("Repetir la capa horizontalmente de forma infinita (necesita un SpriteRenderer en este objeto o en sus hijos).")]
    [SerializeField] private bool loopHorizontal = false;

    private Vector3 startPosition;
    private Vector3 cameraStartPosition;
    private float spriteWidth;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
            cameraTransform = Camera.main.transform;

        startPosition = transform.position;
        cameraStartPosition = cameraTransform.position;

        if (loopHorizontal)
        {
            var sr = GetComponentInChildren<SpriteRenderer>();
            if (sr != null) spriteWidth = sr.bounds.size.x;
        }
    }

    private void OnEnable()  => CinemachineCore.CameraUpdatedEvent.AddListener(OnCameraUpdated);
    private void OnDisable() => CinemachineCore.CameraUpdatedEvent.RemoveListener(OnCameraUpdated);

    // Se llama cada vez que el CinemachineBrain termina de posicionar la cámara.
    private void OnCameraUpdated(CinemachineBrain brain)
    {
        if (cameraTransform == null) return;

        Vector3 camDelta = cameraTransform.position - cameraStartPosition;

        transform.position = new Vector3(
            startPosition.x + camDelta.x * parallaxFactor.x,
            startPosition.y + camDelta.y * parallaxFactor.y,
            startPosition.z);

        if (loopHorizontal && spriteWidth > 0f)
        {
            // Si la cámara se alejó más de un ancho de sprite, recolocamos la capa
            if (cameraTransform.position.x - transform.position.x > spriteWidth)
                startPosition.x += spriteWidth;
            else if (transform.position.x - cameraTransform.position.x > spriteWidth)
                startPosition.x -= spriteWidth;
        }
    }
}
