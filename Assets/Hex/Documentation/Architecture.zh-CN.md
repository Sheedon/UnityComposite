# 架构说明

## 冻结的 v1 设计决策

- 该库提供机制层能力，具体的地形、移动和玩法规则由消费方的游戏模块负责实现。
- 仅支持尖顶六边形（pointy-top hex）。
- 轴向坐标 `Q/R` 为规范表示，立方坐标 `S` 由计算得出。
- 方向值固定为 `NE = 0`、`E = 1`、`SE = 2`、`SW = 3`、`W = 4`、`NW = 5`。
- 运行时代码由一个 `Sheedon.Hex` 程序集组成，逻辑上分为 `Core`、`Map` 和 `Algorithms` 三个区域。
- 测试由一个 `Sheedon.Hex.Tests` 程序集负责。
- 运行时代码不得引用 `UnityEngine`。
- 有限地图成员资格（finite-map membership）和可通行性（traversability）是两个独立概念。
- 成本（cost）为正整数，其业务单位属于消费方定义。

## 依赖方向

`Map` 和 `Algorithms` 可以依赖 `Core`。算法必须通过图结构和遍历契约来操作，而不是直接依赖具体的地图类型。

`IHexGraph` 暴露节点成员资格以及 Hex Distance 为 1 的直接邻居。`IHexTraversalRule` 独立判断一条有向边是否允许通过，并返回严格大于 0 的整数 Cost。`RegionHexGraph` 是从实时 `HexRegion` 到图契约的窄适配器。

路径搜索统一返回 `HexPathResult`，状态包括 `Success`、`NoPath`、`InvalidStart` 和 `InvalidGoal`。BFS 最小化边数，Dijkstra 与 A* 最小化注入的 Cost；A* 默认使用 Hex Distance，也允许提供非负且可采纳的估算回调。CostRange 保存预算内每个可达节点的最小 Cost，FloodFill 返回遵守通行规则的完整连通分量。

## 明确不属于该包的职责

地形、高程、资源、所有权、渲染、网格、着色器、GameObject、拾取、选择、区块流式加载、地图保存、异步调度，以及游戏特定的移动规则都不属于这个包的职责范围。

## 里程碑

- `0.1.0`：Core
- `0.2.0`：有限区域、形状和通用层
- `0.3.0`：图契约和搜索算法
- `0.4.0`：完整测试与公共 API 审查
- `0.5.0`：通过第一个真实 `Dao.Map` 消费方进行验证
