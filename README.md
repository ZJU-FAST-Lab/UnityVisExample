# UnityVisExample

## STEP1 安装Unity3D编辑器

在[unity3D官网](https://unity.cn/releases)下载 Unity-Hub。需要注册Unity账号以获得 personal free license 。

打开 Unity-Hub, 在左侧Install栏目安装Unity编辑器，建议安装2021年的LTS版本。

## STEP2 运行EGO_planner

在 Unity-Hub 的Projects栏目选择[打开工程]，打开仓库中的 UAV_sample 工程。

开启一个新的终端，启动TCP_Connector：
```
cd ROS-Unity_bridge &
catkin_make &
source devel/setup.bash &
roslaunch ros_tcp_endpoint endpoint.launch  
```

开启一个新的终端，启动EGO_planner:
```
cd EGO-Planner-v2 &
catkin_make &
source devel/setup.bash &
roslaunch ego_planner single_drone_interactive.launch  
```

点击运行模拟，并且切换到场景视角。
<img src=https://raw.githubusercontent.com/LanternW/imgs_cat/main/run_and_scene.png width=100%/>

使用3D nav goal工具在rviz中点击目标点，即可实时在unity中运行。
<img src=https://raw.githubusercontent.com/LanternW/imgs_cat/main/run.png width=100% />

## 自定义

选择所需的传感器，在右侧可以编辑传感器的属性。

注意：如果在运行时更改参数，停止运行后不会保存期间的任何修改。

<img src=https://raw.githubusercontent.com/LanternW/imgs_cat/main/setting.png width=100% />


### 关于地图编辑

地图中的物体，一般具有renderer组件和collider组件。前者提供物体的外观，后者提供碰撞检测。

只有renderer的物体，具有外观，但是不能反射产生雷达点云。
只有collider的物体，外观隐形，但是可以产生雷达点云。

<img src=https://raw.githubusercontent.com/LanternW/imgs_cat/main/map.png  width=100% />
