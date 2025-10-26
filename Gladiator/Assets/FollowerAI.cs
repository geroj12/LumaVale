using UnityEngine;

public class FollowerAI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private OrbsHandler orbsHandler;
    [SerializeField] private FollowerVision vision;
    [SerializeField] private FollowerAnimator followerAnimator;
    [SerializeField] private ParticleSystem stateParticles;

    [Header("Settings")]
    [SerializeField] private float followRadius = 2.5f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float baseOrbitSpeed = 30f;
    [SerializeField] private float orbitBoostMultiplier = 1.5f;

    private FollowerState state = FollowerState.Idle;
    private Transform currentTarget;

    private ParticleSystem.MainModule particleMain;
    private ParticleSystem.EmissionModule particleEmission;

    private void Awake()
    {
        if (stateParticles != null)
        {
            particleMain = stateParticles.main;
            particleEmission = stateParticles.emission;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case FollowerState.Idle:
                HandleIdle();
                break;
            case FollowerState.Collecting:
                HandleCollecting();
                break;
            case FollowerState.Returning:
                HandleReturning();
                break;
        }

        UpdateParticleVisuals();
    }

    private void HandleIdle()
    {
        OrbitAroundPlayer();

        if (vision.detectedEnergy != null)
        {
            currentTarget = vision.detectedEnergy;
            state = FollowerState.Collecting;
            followerAnimator.SetMoving(true);
        }
    }

    private void HandleCollecting()
    {
        if (currentTarget == null)
        {
            state = FollowerState.Returning;
            return;
        }

        MoveTowards(currentTarget.position);

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance < 1f)
        {
            if (currentTarget.CompareTag("EnergyOrb"))
            {
                if (orbsHandler != null)
                    orbsHandler.AddOrb();

                Destroy(currentTarget.gameObject);
            }

            currentTarget = null;
            state = FollowerState.Returning;
        }
    }

    private void HandleReturning()
    {
        if (player == null) return;

        MoveTowards(player.position);

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < followRadius)
        {
            state = FollowerState.Idle;
            followerAnimator.SetMoving(false);
        }
    }

    private void MoveTowards(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        transform.position += dir * moveSpeed * Time.deltaTime;
        transform.forward = dir;
    }

    private void OrbitAroundPlayer()
    {
        if (player == null) return;

        float orbitSpeed = baseOrbitSpeed;

        // 🔥 Boost wenn Max Orbs erreicht sind
        if (orbsHandler != null && orbsHandler.CurrentOrbs >= orbsHandler.MaxOrbs)
            orbitSpeed *= orbitBoostMultiplier;

        transform.RotateAround(player.position, Vector3.up, orbitSpeed * Time.deltaTime);

        float hover = Mathf.Sin(Time.time * 2f) * 0.5f;
        Vector3 desiredPos = (transform.position - player.position).normalized * followRadius + player.position + new Vector3(0, hover, 0);
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 2f);
    }

    private void UpdateParticleVisuals()
    {
        if (stateParticles == null) return;

        Color color = Color.cyan; // default Idle
        float emissionRate = 15f; // base emission

        // 1️⃣ Feind in Sicht → Rot leuchten
        if (vision != null && vision.detectedEnemy != null)
        {
            color = Color.red;
            emissionRate = 40f;
        }
        else
        {
            // 2️⃣ Beim Sammeln → Weiß
            if (state == FollowerState.Collecting)
            {
                color = Color.white;
                emissionRate = 30f;
            }

            // 3️⃣ Max Orbs → Pulsieren (Emission schwankt)
            if (orbsHandler != null && orbsHandler.CurrentOrbs >= orbsHandler.MaxOrbs)
            {
                float pulse = (Mathf.Sin(Time.time * 4f) + 1f) * 0.5f; // 0..1
                emissionRate = Mathf.Lerp(15f, 60f, pulse); // pulsierende Stärke
                color = Color.cyan; // Farbe bleibt, nur Stärke pulsiert
            }
        }

        particleMain.startColor = color;
        particleEmission.rateOverTime = emissionRate;
    }
}
