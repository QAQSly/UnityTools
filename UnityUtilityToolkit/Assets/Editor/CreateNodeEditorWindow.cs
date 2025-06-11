using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Utils;
using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Sly
{
   
    public class ItemData
    {
        public string itemName;
        public int itemCount = 1;
    }
    public class ItemNode : Node {
        public ItemData itemData;
        public Port inputPort;
        public Port outputPort;
        public Action<ItemNode> OnNodeSelected;

        public ItemNode(ItemData data) {
            itemData = data;
            title = $"{itemData.itemName} {itemData.itemCount}";
        
            // 输入端口（用于合成节点连接）
            inputPort = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            inputPort.portName = "输入";
            inputContainer.Add(inputPort);
        
            // 输出端口（用于合成节点连接）
            outputPort = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            outputPort.portName = "输出";
            outputContainer.Add(outputPort);
            
        }

        public Port GetInputPort()
        {
            if (inputPort == null)
            {
                Debug.Log("当前输入节点为空");
            }
            return inputPort;
        }

        public Port GetOutputPort()
        {
            if (outputPort == null)
            {
                Debug.Log("当前输出节点为空");
            }
            return outputPort;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            Debug.Log($"当前节点被点击  {this.title}");
        }
        
    }
    
    public class EdgeConnectorListener : IEdgeConnectorListener
    {
        private GraphView _graphView;

        public EdgeConnectorListener(GraphView graphView)
        {
            this._graphView = graphView;
        }

        
        // 当用户开始拖拽连线时调用
        public void OnDropOutsidePort(Edge edge, Vector2 position)
        {
            Debug.Log("开始连线");
            Debug.Log($"尝试连线: input={edge.input?.node?.title}, output={edge.output?.node?.title}");
            if (edge.input == null)
            {
                Debug.Log("输入为空删除连线");
                _graphView.RemoveElement(edge);
            }
            // 获取档期按选中节点
            // var selectedNodes =  

            if (edge.output == null)
            {
                Debug.Log("输出为空删除连线");
                _graphView.RemoveElement(edge); 
            }
        }

        // 当用户释放鼠标完成连线时调用
        public void OnDrop(GraphView graphView, Edge edge)
        {
            if (edge.input == null || edge.output == null) {
                Debug.LogError($"连线失败: input={edge.input?.portName}, output={edge.output?.portName}");
                graphView.RemoveElement(edge);
                return;
            }
            // 检查方向是否合法（Input -> Output 或 Output -> Input）
            if (edge.input.direction == edge.output.direction) {
                Debug.LogError("不能连接同方向的端口");
                graphView.RemoveElement(edge);
                return;
            }

            // 正式连接
            edge.input.Connect(edge);
            edge.output.Connect(edge);
            graphView.AddElement(edge);
            // 输入节点
            ItemNode fromNode = edge.input.node as ItemNode;
            ItemNode toNode = edge.output.node as ItemNode;
            Debug.Log($"连线成功！{fromNode.title} -> {toNode.title}");
        }
    }

   
    public class CraftingGraphView : GraphView
    {
        Vector2 position = Vector2.zero;
        ContentZoomer _cz = null;
        private ContentDragger _cd = null;
        private SelectionDragger _sd = null;
        private bool _enableAutoConnect = true; // 是否启用自动连线
        EdgeConnectorListener edgeConnectorListener;
        private ItemNode _lastCreatedNode; // 记录上一次创建的节点
        public CraftingGraphView()
        {
            this.RegisterCallback<MouseUpEvent>(OnMouseUp);
            Insert(0, new GridBackground());
            _cz = new ContentZoomer();
            _cd = new ContentDragger();
            _sd = new SelectionDragger();
            this.AddManipulator(_cz);
            this.AddManipulator(_cd);
            this.AddManipulator(_sd);
            // 框选支持
            this.AddManipulator(new RectangleSelector());
            edgeConnectorListener = new EdgeConnectorListener(this);
            this.AddManipulator(new EdgeManipulator());
            // 启用连线功能
            this.AddManipulator(new ContextualMenuManipulator(AddContextMenu));
            SetupEdgeHandling(); 
        }
        
        public void AddContextMenu(ContextualMenuPopulateEvent menuEvent)
        {
            position = menuEvent.mousePosition;
            menuEvent.menu.AppendAction(
                _enableAutoConnect ? "禁用自动连线" : "启用自动连线",
                _ => _enableAutoConnect = !_enableAutoConnect
            );
            menuEvent.menu.AppendAction("添加物品节点", _ => CreateItemNode());
        }
        private void OnMouseUp(MouseUpEvent evt) {
            // 确保鼠标释放时正确处理连线
            Debug.Log("鼠标释放事件触发");
        }
        public event Action<Node> nodeCreated;

        
        private void SetupEdgeHandling()
        {
            // 1. 初始化全局连线支持

            // 2. 创建监听器
           

            // 3. 为所有现有节点绑定连线器（修复问题1）
            foreach (var node in nodes.ToList()) // 遍历已存在的节点
            {
                if (node is ItemNode itemNode)
                {
                    AttachPortListeners(itemNode, edgeConnectorListener);
                }
            }
            // 4. 监听新节点（保持原有逻辑）
            nodeCreated += node =>
            {
                if (node is ItemNode itemNode)
                {
                    AttachPortListeners(itemNode, edgeConnectorListener);
                }
            };
        }

// 辅助方法：为节点的输入/输出端口绑定监听器
        private void AttachPortListeners(ItemNode itemNode, EdgeConnectorListener listener)
        {
            if (itemNode.inputPort != null)
            {
                Debug.Log("输入连线初始化");
                itemNode.inputPort.AddManipulator(new EdgeConnector<Edge>(listener));
                itemNode.inputPort.RegisterCallback<MouseDownEvent>(e => Debug.Log("输入端口被点击"));
            }
            if (itemNode.outputPort != null)
            {
                Debug.Log("输出连线初始化");
                itemNode.outputPort.AddManipulator(new EdgeConnector<Edge>(listener));
                itemNode.outputPort.RegisterCallback<MouseDownEvent>(e => Debug.Log("输出端口被点击"));
            }
        }
        
        public void CreateItemNode()
        {
            Debug.Log($"传入的鼠标的位置 {position}");
            ItemNode node = new ItemNode(new ItemData { itemName = "物品", itemCount = 1 });
            node.SetPosition(new Rect(position, new Vector2(200, 100)));
            this.AddElement(node);
            if (_lastCreatedNode != null && _enableAutoConnect)
            {
                ConnectNodesAutomatically(_lastCreatedNode, node);
            }

            _lastCreatedNode = node; // 更新上一次创建的节点
            nodeCreated?.Invoke(node);
        }
        
        private void ConnectNodesAutomatically(ItemNode fromNode, ItemNode toNode)
        {
            Edge edge = new Edge
            {
                output = fromNode.outputPort,
                input = toNode.inputPort
            };

            // 添加连线到 GraphView
            edge.output.Connect(edge);
            edge.input.Connect(edge);
            this.AddElement(edge);

            Debug.Log($"自动连线: {fromNode.title} -> {toNode.title}");
        }

      
        
        // 判断点相连
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            Debug.Log("端口开始相连");
            return ports.ToList().Where(endPort => endPort.direction != startPort.direction
                                                   && endPort.node != startPort.node).ToList();
        }

        private void AddEdgeByPorts(Port _outputPort, Port _inputPort)
        {
            if (_outputPort.node == _inputPort.node)
            {
                Debug.LogError("输入输出节点相同");
                return;
            }
            Edge tempEdge = new Edge()
            {
                output = _outputPort,
                input = _inputPort
            };
            tempEdge.input.Connect(tempEdge);
            tempEdge.output.Connect(tempEdge);
            Add(tempEdge);
        }
        

    }

      
    [InitializeOnLoad]
    public class CreateNodeEditorWindow : EditorWindow
    {
        private GraphView _graphView;
        [MenuItem("SlyTools/节点编辑工具")]
        public static void ShowWindow()
        {
            GetWindow<CreateNodeEditorWindow>().Show();
        }

        private void OnEnable()
        {
            _graphView = new CraftingGraphView();
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void OnGUI()
        {
        }
    }
}