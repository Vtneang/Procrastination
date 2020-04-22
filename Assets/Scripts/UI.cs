using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI : MonoBehaviour
{

    [Header("Public Variables")]
    public RectTransform HealthBar;
    public GameObject player;


    private float m_healthbar;
    private float m_healthbary;
    private float c_healthbar;
    private float playerHealth;

    // Start is called before the first frame update
    void Start()
    {
        c_healthbar = m_healthbar = HealthBar.localScale.x;
        m_healthbary = HealthBar.localScale.y;
        playerHealth = player.GetComponent<Player>().m_Health;
    }

    //Changes healthbar based off AMOUNT LEFT
    public void LoseHealth(float amount)
    {
        c_healthbar = (amount / playerHealth) * m_healthbar;
        HealthBar.localScale = new Vector2(c_healthbar, m_healthbary);
    }
}
