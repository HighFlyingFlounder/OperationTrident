using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface NetSyncInterface
{
    /// <summary> 
    /// 网络同步传入数据，改变该脚本组件状态        
    /// </summary> 
    /// <param name="data">传入的数据</param>         
    /// <returns></returns> 
    void SetData(SyncData data);
    /// <summary> 
    /// 获取该脚本组件的状态，自动同步到其他客户端中有着该对象名字的gameObject中        
    /// </summary> 
    /// <param></param>         
    /// <returns type="SyncData">返回打包好的数据</returns> 
    SyncData GetData();

    void Init(NetSyncController controller);
    //Hashtable Variables { get; set; }
}
