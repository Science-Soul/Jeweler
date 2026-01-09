using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Instrument : MonoBehaviour
{
    [SerializeField] Texture2D cursorTexture;
    protected bool isMobile = false;

    protected AnvilPiece currentAnvilTarget;
    protected Level level;
    public const int BASE_CHARGE = 18;
    [SerializeField] private GameObject stand;

    [SerializeField] protected GameObject instrumentFrame;

    public virtual int INSTRUMENT_CHARGE
    {
        get
        {
            return 0;
        }
        set { }
    }

    public ParticleSystem spriteOutlinePrefab;

    protected bool isGrabbed;
    protected static bool isInstrumentActive;
    protected CircleCollider2D col;
    protected Vector3 originPos;
    protected ParticleSystem spriteOutline;
    protected Tween instrumentTween;

    public GameObject outlineCrystal;


    virtual protected void Awake()
    {
        level = GameObject.FindGameObjectWithTag("Level").GetComponent<Level>();
        instrumentFrame.SetActive(false);
        isMobile = false;
    }

    // Start is called before the first frame update
    virtual protected void Start()
    {
        isGrabbed = false;
        col = GetComponent<CircleCollider2D>();
        originPos = transform.position;
        spriteOutline = Instantiate(spriteOutlinePrefab, 9999 * Vector3.one, Quaternion.identity);
        spriteOutline.gameObject.SetActive(false);
    }

    // Update is called once per frame
    virtual protected void Update()
    {
        if (level.grid.gameOver)
        {
            return;
        }

        if (isGrabbed)
        {
            if (!Application.isMobilePlatform && !isMobile)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                mousePos.z = 0;
                transform.position = mousePos;

                if (Input.GetMouseButtonDown(1) || Input.GetKeyDown(KeyCode.Escape))
                {
                    ResetInstrument();
                }
            }
        }
    }



    protected void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            SelectTarget(other);
        }
    }

    protected void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(tag))
        {
            DeselectTarget();
        }
    }

    private void OnMouseEnter()
    {
        outlineCrystal.SetActive(true);
        outlineCrystal.transform.position = this.transform.position;
    }

    private void OnMouseExit()
    {
        outlineCrystal.SetActive(false);
    }

    protected void OnMouseDown()
    {
        if (!isGrabbed && !isInstrumentActive)
        {
            isGrabbed = true;
            isInstrumentActive = true;

            if (!Application.isMobilePlatform && !isMobile)
            {
                ShrinkCollider();
                //Cursor.visible = false;

                if (stand != null)
                {
                    stand.transform.SetParent(null);
                }
                Camera.main.eventMask &= ~LayerMask.GetMask("Pieces");
            }
            else
            {
                if (isInstrumentActive)
                {
                    Debug.Log("Mobile instrument activeted");
                    MobileInstrumentActivate();
                }
            }
        }
        else if (Application.isMobilePlatform || isMobile)
        {
            Debug.Log("All mobile instruments reset");
            AllMobileInstrumentReset();
        }
    }

    protected void ResetInstrument()
    {
        isGrabbed = false;
        isInstrumentActive = false;

        transform.position = originPos;
        if (stand != null)
        {
            stand.transform.SetParent(transform);
        }
        ExpandCollider();
        //Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
        //Cursor.visible = true;
        col.enabled = true;

        Camera.main.eventMask |= LayerMask.GetMask("Pieces");

    }

    virtual protected void ShrinkCollider()
    {
        col.offset = new Vector2(-0.41f, 0.41f);
        col.radius = 0.0001f;
    }

    virtual protected void ExpandCollider()
    {
        col.offset = Vector2.zero;
        col.radius = 0.69f;
    }

    protected virtual void SelectTarget(Collider2D col)
    {
        currentAnvilTarget = col.gameObject.GetComponent<AnvilPiece>();
        currentAnvilTarget.isSelected = true;
        spriteOutline.transform.position = col.gameObject.GetComponent<BoxCollider2D>().bounds.center;
        spriteOutline.gameObject.SetActive(true);
    }

    protected virtual void DeselectTarget()
    {
        spriteOutline.gameObject.SetActive(false);

        if (currentAnvilTarget != null)
        {
            currentAnvilTarget.isSelected = false;
        }
    }


    protected void UseInstrumentCharge()
    {
        level.currentTotalPiecesCollected -= INSTRUMENT_CHARGE;
        level.hud.SetInstrumentBar();
        ResetInstrument();
        CheckAllInstrumentsCharge();
    }

    protected void TurnOffTheInstrument()
    {
        ResetInstrument();
        this.gameObject.SetActive(false);
    }

    public void CheckAllInstrumentsCharge()
    {
        foreach (GameObject go in level.instruments)
        {
            if (level.currentTotalPiecesCollected < go.GetComponent<Instrument>().INSTRUMENT_CHARGE)
            {
                go.SetActive(false);
            }
        }
    }

    protected GameObject CheckTargetTagTap(Vector3 inputPosition, string targetLayer, string targetTag)
    {
        RaycastHit2D hit = Physics2D.Raycast(inputPosition, Vector2.zero, 0f, LayerMask.GetMask(targetLayer));

        if (hit.collider != null && hit.collider.CompareTag(targetTag))
        {
            // Попадание гарантировано произошло на нужном слое "Interactive"
            Debug.Log($"2D: Попали в объект на слое {targetTag}: " + hit.collider.gameObject.name);
            return hit.collider.gameObject;
        }
        else
        {
            Debug.Log($"2D: Либо промах, либо объект не на слое {targetTag}.");
            return null;
        }
    }

    protected void MobileInstrumentActivate()
    {
        instrumentFrame.SetActive(true);
    }

    protected void AllMobileInstrumentReset()
    {
        isInstrumentActive = false;
        foreach (GameObject go in level.instruments)
        {
            Instrument instrument = go.GetComponent<Instrument>();
            instrument.instrumentFrame.SetActive(false);
            instrument.isGrabbed = false;
        }
    }
}
