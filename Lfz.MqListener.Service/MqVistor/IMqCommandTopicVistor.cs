// /*======================================================================
// *
// *        Copyright (C)  1996-2012  lfz    
// *        All rights reserved
// *
// *        Filename :IMqCommandVistor.cs
// *        DESCRIPTION :
// *
// *        Created By 林芳崽 at 2016-01-20 17:17
// *        https://git.oschina.net/lfz
// *
// *======================================================================*/

using Lfz.MqListener.Mq;
using Lfz.MqListener.Shared.Models;

namespace Lfz.MqListener.MqVistor
{
    /// <summary>
    /// 单例模式命令访问器
    /// </summary>
    public interface IMqCommandTopicVistor : ISingletonDependency
    {
        /// <summary>
        /// 主题处理程序
        /// </summary>
        /// <param name="topicName"></param>
        /// <param name="commandInfo"></param>
        void Vistor(TopicName topicName, MqCommandInfo commandInfo);
    }
}