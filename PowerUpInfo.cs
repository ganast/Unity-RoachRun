using UnityEngine;

public class PowerUpInfo : MonoBehaviour {

    [SerializeField]
    private float Bonus = 10.0f;

    public float GetBonus() {
        return Bonus;
    }
}
