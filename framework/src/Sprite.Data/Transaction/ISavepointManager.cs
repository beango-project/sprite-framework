namespace Sprite.Data.Transaction
{
    /// <summary>
    /// 保存点管理器
    /// </summary>
    public interface ISavepointManager
    {
        /// <summary>
        /// 创建保存点
        /// </summary>
        /// <param name="savepointName">保存点的名称</param>
        void CreateSavepoint(string savepointName);

        /// <summary>
        /// 回到指定的保存点
        /// </summary>
        /// <remarks>
        /// 该保存点会被自动释放
        /// </remarks>
        /// <param name="savepoint">要回到的保存点</param>
        void RollbackToSavepoint(string savepoint);

        /// <summary>
        /// 显示释放指定的保存点
        /// </summary>
        /// <param name="savepoint"></param>
        void ReleaseSavepoint(string savepoint);
    }
}