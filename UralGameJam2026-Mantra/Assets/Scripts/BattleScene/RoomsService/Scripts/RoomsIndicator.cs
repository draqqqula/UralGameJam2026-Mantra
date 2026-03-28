using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class RoomsIndicator : MonoBehaviour
{
    [SerializeField] private List<RoomNode> _roomsNodes = new List<RoomNode>();
    [SerializeField] private HorizontalLayoutGroup _horizontalLayoutGroup;
    
    [SerializeField] private Sprite _currentRoomImage;
    [SerializeField] private Sprite _prevRoomImage;
    [SerializeField] private Sprite _bossRoomImage;
    
    [SerializeField] private RectTransform _content;
    [SerializeField] private RectTransform _viewport;
    
    private RoomsController _roomsController;
    [SerializeField] private Image _nodePrefab;
    
    [SerializeField] private int _countInViewport;
    private float _spaceOffset;
    
    private float _viewportWidth;
    private float _roomWidth;

    private void Awake()
    {
        _roomsController = ServiceLocator.Instance.GetService<RoomsController>();
        _roomsController.OnRoomUpdated += UpdateRoom;
        
        for (int i = 0; i < _roomsController.RoomsCount; i++)
        {
            var image = Instantiate(_nodePrefab, _content);
            _roomsNodes.Add(new RoomNode() {Image = image, Index = i});
            
            if (i == _roomsController.RoomsCount - 1) image.sprite = _bossRoomImage;
        }
        
        SetCurrentNode(_roomsNodes[0]);
        
        _roomWidth = _roomsNodes[0].Image.rectTransform.rect.width;
        _viewportWidth = _viewport.rect.width;
        
        AlignElements(_countInViewport);
    }

    private void AlignElements(int countInViewport)
    {
        var roomsCount = _roomsNodes.Count;
        
        if (roomsCount < countInViewport)
        {
            countInViewport = roomsCount;
        }

        if (roomsCount <= 1)
        {
            _spaceOffset = 0;
        }
        else
        {
            var freeSpace = _viewportWidth - (countInViewport * _roomWidth);
            _spaceOffset = freeSpace / (countInViewport - 1);
        }

        _horizontalLayoutGroup.spacing = _spaceOffset;
    }

    [ContextMenu("Update Room")]
    public void UpdateRoom()
    {
        var currentNode = GetCurrentNode();
        if (currentNode.Index + 1 >= _roomsNodes.Count) return;
        
        UpdateRoom(currentNode.Index + 1);
    }

    private void UpdateRoom(int roomIndex)
    {
        var currentNode = GetCurrentNode();
        
        currentNode.IsCurrent = false;
        currentNode.Image.sprite = _prevRoomImage;
        
        var newNode = _roomsNodes[roomIndex];
        SetCurrentNode(newNode);
        SlideIndicator(newNode);
    }

    private void SlideIndicator(RoomNode currentNode)
    {
        var content = _content;

        Vector3 worldPos = currentNode.Image.rectTransform.position;
        Vector3 localPos = _viewport.InverseTransformPoint(worldPos);
        
        float halfWidth = _viewportWidth / 2f;
        
        if (localPos.x > halfWidth)
        {
            float offset = localPos.x - halfWidth;
            
            var contentPos = content.anchoredPosition;
            contentPos.x -= offset;
            contentPos.x = Mathf.Clamp(contentPos.x, -(_content.rect.width - _viewportWidth), 0);
            content.anchoredPosition = contentPos;
        }
    }

    private RoomNode GetCurrentNode()
    {
        return _roomsNodes.FirstOrDefault(r => r.IsCurrent);
    }

    private void SetCurrentNode(RoomNode currentNode)
    {
        currentNode.IsCurrent = true;
        currentNode.Image.sprite = _currentRoomImage;
    }

    private void OnDestroy()
    {
        _roomsController.OnRoomUpdated -= UpdateRoom;
    }
}

[Serializable]
public class RoomNode
{
    public int Index;
    public Image Image;
    public bool IsCurrent;
}


