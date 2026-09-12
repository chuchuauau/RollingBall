using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed;
    public float movementSpeed;

    public int score;
    public TextMeshProUGUI scoreTxt;

    public Renderer playerRenderer;
    public float dissolveSpeed = 1.5f;
    private float dissolveAmount = 0f;
    private bool isDissolving = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerRenderer == null) playerRenderer = GetComponent<Renderer>();

        if (playerRenderer != null) playerRenderer.material.SetFloat("_DissolveAmount", 0f);
    }

    void Update()
    {
        if (isDissolving)
        {
            dissolveAmount += Time.deltaTime * dissolveSpeed;
            if (playerRenderer != null)
            {
                playerRenderer.material.SetFloat("_DissolveAmount", Mathf.Clamp01(dissolveAmount));
            }
            rb.velocity = Vector3.zero;
            return;
        }

        float movementInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector3(speed, rb.velocity.y, -movementInput * movementSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            score++;

            if (scoreTxt != null)
            {
                scoreTxt.text = score.ToString();
            }

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UpdateScoreUI(score);
            }

            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && !isDissolving)
        {
            isDissolving = true;

            if (GameManager.Instance != null)
            {
                GameManager.Instance.ShowGameOver();
            }
            else
            {
                GameManager gm = FindObjectOfType<GameManager>();
                if (gm != null) gm.ShowGameOver();
            }
        }
    }
}