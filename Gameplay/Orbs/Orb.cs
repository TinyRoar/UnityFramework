using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

public class Orb : MonoBehaviour
{
    [SerializeField]
    private float startForce = 5;

    [SerializeField]
    private float distanceToBeFinished = 5.0f;

    [SerializeField]
    private float force = 100;

    public Vector3 targetPos { get; set; }

    public Vector3 startPos { get; set; }

    private Rigidbody2D rigid;
    private TrailRenderer trailRenderer;
    private float forcex;
    private float forcey;
    private Vector3 dir;

    private bool tapped = false;
    private InitManager _initManager;

    void Awake()
    {
        this._initManager = InitManager.Instance;
    }

    void OnEnable()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        if (trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.enabled = false;
        }

        dir = Vector3.zero;
        tapped = false;
        rigid.velocity = Vector3.zero;
        force = startForce;

        Vector3 rand = Quaternion.Euler(0,0,RandomGenerator.Instance.Range(0,361)) * Vector3.right * force;
        Timer.Instance.Add(0.1f, delegate
        {
            rigid.AddForce(rand, ForceMode2D.Impulse);
        });

        Timer.Instance.Add(10f, delegate
        {
            tapped = true;
        });

        Timer.Instance.Add(1f, () => trailRenderer.enabled = true);

        InvokeRepeating("MoveToRandomDir", 1f, 1f);
    }

    void OnDisable()
    {
        trailRenderer.enabled = false;
    }

    void Update()
    {
        if (tapped)
        {
            dir = (targetPos - transform.position).normalized;
            force += 2;

            rigid.MovePosition(transform.position + dir * force * Time.deltaTime);
        }
        else
        {
            dir = (startPos - transform.position).normalized;

            if(this._initManager.Debug)
                Debug.DrawRay(transform.position, dir *10, Color.red);

            if (Vector3.Distance(startPos, transform.position) >= 5)
                rigid.AddForce(dir * force, ForceMode2D.Force);
            else
                rigid.AddForce(dir * -1 * force, ForceMode2D.Force);
        }

        var distance = Vector2.Distance(transform.position, targetPos);

        if (distance < this.distanceToBeFinished)
        {
            // execute what to do if goal reached
            // TODO add delegate/event

            ObjectPool.Instance.Deactivate(gameObject);
        }
    }

    // OnMouseDown is called when the user has pressed the mouse button while over the GUIElement or Collider
    public void OnMouseDown()
    {
        rigid.velocity = Vector3.zero;
        tapped = true;
    }


    public void OnMouseOver()
    {
        rigid.velocity = Vector3.zero;
        tapped = true;
    }

    void MoveToRandomDir()
    {
        rigid.AddForce(Quaternion.Euler(0, 0, RandomGenerator.Instance.Range(0, 361)) * Vector3.right * force, ForceMode2D.Force);
    }


}
