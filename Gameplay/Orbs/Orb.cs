using UnityEngine;
using System.Collections;
using TinyRoar.Framework;

/// <summary>
/// Important: GameObject of this Orb Script must be disabled by defaule!!!1!1eleven
/// </summary>
public class Orb : MonoBehaviour
{
    [SerializeField]
    private float startForce = 5;

    [SerializeField]
    private float distanceToBeFinished = 5.0f;

    [SerializeField]
    private float force = 100;

    /// <summary>
    /// fly directly to goal or stay nearby startPos and react on Click/Tap
    /// </summary>
    [SerializeField]
    private bool flyDirectlyToGoal = false;

    [SerializeField]
    private float startFlyingAfterSeconds = 10.0f;

    [SerializeField]
    private bool useTrailRenderer = true;

    public Vector3 targetPos { get; set; }

    public Vector3 startPos { get; set; }

    private bool FlyToGoal
    {
        get
        {
            return _flyToGoal;
        }
        set {
            _flyToGoal = value;
            if(value && useTrailRenderer)
                trailRenderer.enabled = true;
        }
    }

    private Rigidbody2D rigid;
    private TrailRenderer trailRenderer;
    private Vector3 dir;
    private bool _flyToGoal = false;
    private InitManager _initManager;

    void Start()
    {
        this._initManager = InitManager.Instance;
    }

    void OnEnable()
    {
        if (rigid == null)
        {
            rigid = GetComponent<Rigidbody2D>();
        }
        if (useTrailRenderer && trailRenderer == null)
        {
            trailRenderer = GetComponent<TrailRenderer>();
            trailRenderer.enabled = false;
        }

        dir = Vector3.zero;
        _flyToGoal = false;
        rigid.velocity = Vector3.zero;
        force = startForce;

        // set/reset position
        this.transform.position = startPos;
        var localPos = this.GetComponent<RectTransform>().localPosition;
        localPos.z = 0;
        this.GetComponent<RectTransform>().localPosition = localPos;

        Vector3 rand = Quaternion.Euler(0,0,RandomGenerator.Instance.Range(0,361)) * Vector3.right * force;
        Timer.Instance.Add(0.1f);

        if (flyDirectlyToGoal)
        {
            FlyToGoal = true;
        }
        else
        {
            Timer.Instance.Add(this.startFlyingAfterSeconds);
        }

        InvokeRepeating("MoveToRandomDir", 1f, 1f);
    }

    void OnDisable()
    {
        if(useTrailRenderer)
            trailRenderer.enabled = false;
    }

    void Update()
    {
        if (_flyToGoal)
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
        FlyToGoal = true;
    }


    public void OnMouseOver()
    {
        rigid.velocity = Vector3.zero;
        FlyToGoal = true;
    }

    void MoveToRandomDir()
    {
        rigid.AddForce(Quaternion.Euler(0, 0, RandomGenerator.Instance.Range(0, 361)) * Vector3.right * force, ForceMode2D.Force);
    }


}
