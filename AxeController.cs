using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AxeController : MonoBehaviour
{
    /* Random Stuff */
    [SerializeField] private LayerMask layerMask;
    
    /* Axe Stats */
    [SerializeField] private float axeSpeed;

    /* Axe Attributes */
    private Transform axeTransform;
    private Rigidbody2D axeRigidbody;
    private Player player;

    // Start is called before the first frame update
    /* Script that throws the axe by reading the mouse angle relative to the player, and throwing based on said angle */
    void Start()
    {
        axeTransform = GetComponent<Transform>();
        axeRigidbody = GetComponent<Rigidbody2D>();
        player = GetComponentInParent<Player>();

        Vector2 mousePos = Input.mousePosition;

        Vector2 playerPos = Camera.main.WorldToScreenPoint(player.transform.localPosition);
        Debug.Log("Mouse: " + mousePos + " Player: " + playerPos);

        // subtracted for offset of mousePos to screen, FIX LATER WHEN SCREENS UPDATED
        float angle = Mathf.Atan2(mousePos.y - playerPos.y, mousePos.x - playerPos.x) * Mathf.Rad2Deg;
        Debug.Log("Angle: " + angle);

        float xcomponent = Mathf.Cos(angle * Mathf.PI / 180) * axeSpeed;
        float ycomponent = Mathf.Sin(angle * Mathf.PI / 180) * axeSpeed;

        axeRigidbody.velocity = new Vector2(xcomponent, ycomponent);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(1))
        {
            axeTeleport();
        }
    }

    /* Gets the first available tp point's transform by checking if the tp collider is not touching a platform */
    private Transform axeTeleportPoint()
    {
        Transform[] tps = new Transform[4];
        bool[] tpValid = new bool[4];
        
        for(int i = 0; i < tps.Length; i++)
        {
            tps[i] = axeTransform.GetChild(i);
            Collider2D collider = tps[i].GetComponent<CircleCollider2D>();
            /*if(tps[i].GetComponent<CircleCollider2D>().IsTouchingLayers(6))
                tpValid[i] = false;*/
            int xPos;
            int yPos;

            switch(i)
            {
                case 0: 
                    xPos = 1;
                    yPos = 1;
                break;

                case 1: 
                    xPos = 1;
                    yPos = -1;
                break;

                case 2: 
                    xPos = -1;
                    yPos = -1;
                break;

                case 3: 
                    xPos = -1;
                    yPos = 1;
                break;

                default:
                    xPos = 0;
                    yPos = 0;
                break;
            }

            if(Physics2D.BoxCast(collider.bounds.center, collider.bounds.size, 0f, 
                new Vector2(xPos, yPos), 1f, layerMask))
                tpValid[i] = false;
            else
                tpValid[i] = true;

            Debug.Log("tp" + i + ": (" + xPos + ", " + yPos + ")");
            Debug.Log("Hit?" + tpValid[i]);
        }

        for(int i = 0; i < tpValid.Length; i++)
        {
            if(tpValid[i])
                return tps[i];
        }

        return null;
    }

    /* Handles teleport function by detecting whether any of the tp points are obstructed
       and teleporting to the first one that's unobstructed in a clockwise circle */
    public void axeTeleport()
    {
        // teleports by switching player position
        // TODO: add animation control here for teleport
        Transform tp = axeTeleportPoint();
        player.transform.position = tp.position;

        // object cleanup and axe retrieval
        player.retrieveAxe();
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D struck)
    {
        if(struck.gameObject.layer == 6)
        {
            axeRigidbody.gravityScale = 0;
            axeRigidbody.velocity = Vector2.zero;
            axeRigidbody.Sleep();
        }

        if(struck.gameObject.layer == 7 && axeRigidbody.IsSleeping())
        {
            player.retrieveAxe();
            Destroy(gameObject);
        }

    }
}
