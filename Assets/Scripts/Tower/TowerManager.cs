using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager>
{
    private TowerBtn _selectedTowerBtn;
    private SpriteRenderer spriteRenderer;
    private List<GameObject> _towerList = new List<GameObject>();
    private List<Collider2D> _fullBuildGrounds = new List<Collider2D>();

    public TowerBtn SelectedTowerBtn { 
        get
        {
            return _selectedTowerBtn;
        }
        set
        {
            _selectedTowerBtn = value;
        }
    }

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        DisableSpriteRenderer();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);


            if(hit.collider.CompareTag("BuildGround") && SelectedTowerBtn != null)
            {
                PlaceTower(hit);
                hit.collider.tag = "BuildGroundFull";
                _fullBuildGrounds.Add(hit.collider);
                DisableSpriteRenderer();
            }
        }
        if (spriteRenderer.enabled == true)
        {
            FollowMouse();
        }
    }

    private void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    private void EnableSpriteRenderer(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = sprite;
    }

    public void DisableSpriteRenderer()
    {
        spriteRenderer.enabled = false;
    }

    public void PressedButton(TowerBtn pressedButton)
    {
        if(pressedButton.TowerPrice <= GameManager.Instance.TotalMoney)
        {
            SelectedTowerBtn = pressedButton;
            EnableSpriteRenderer(SelectedTowerBtn.TowerObject.GetComponent<SpriteRenderer>().sprite);
        }
    }

    private void PlaceTower(RaycastHit2D hit2D)
    {
        // Imlec UI elemani uzerinde degilse
        if(!EventSystem.current.IsPointerOverGameObject() && SelectedTowerBtn != null)
        {
            GameObject newTower = Instantiate(SelectedTowerBtn.TowerObject) as GameObject;
            newTower.transform.position = hit2D.transform.position;
            BuyTower(SelectedTowerBtn.TowerPrice);
            RegisterTower(newTower);
            GameManager.Instance.AudioSource.PlayOneShot(SoundManager.Instance.Towerbuilt);
        }
        SelectedTowerBtn = null;
    }

    private void BuyTower(int price)
    {
        GameManager.Instance.SubtractMoney(price);
    }

    private void RegisterTower(GameObject tower)
    {
        _towerList.Add(tower);
    }


    public void DestroyAllTowers()
    {
        foreach(GameObject tower in _towerList)
        {
            Destroy(tower);
        }
        foreach(Collider2D collider in _fullBuildGrounds)
        {
            collider.tag = "BuildGround";
        }
        _fullBuildGrounds.Clear();
        _towerList.Clear();
    }
}
