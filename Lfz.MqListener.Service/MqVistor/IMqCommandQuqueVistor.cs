using Lfz.MqListener.Mq;
using Lfz.MqListener.Shared.Models;

namespace Lfz.MqListener.MqVistor
{
    /// <summary>
    /// 单例模式命令访问器
    /// </summary>
    public interface IMqCommandQuqueVistor : ISingletonDependency
    {
        /// <summary>
        /// 队列处理程序
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="commandInfo"></param>
        void Vistor(QuqueName queueName, MqCommandInfo commandInfo);
    }
}