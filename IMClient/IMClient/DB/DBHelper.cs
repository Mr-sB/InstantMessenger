using System;
using Android.Content;
using Android.Database;
using Android.Database.Sqlite;
using Android.Runtime;

namespace IMClient.DB
{
    /// <summary>
    /// SQLite数据库创建支持的数据类型： 整型数据、字符串类型、日期类型、二进制
    /// </summary>
    public class DBHelper : SQLiteOpenHelper
    {
        public DBHelper(Context context, string name, int version) : base(context, name, null, version)
        {
        }
        
        public DBHelper(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public DBHelper(Context context, string name, SQLiteDatabase.ICursorFactory factory, int version) : base(context, name, factory, version)
        {
        }

        public DBHelper(Context context, string name, SQLiteDatabase.ICursorFactory factory, int version, IDatabaseErrorHandler errorHandler) : base(context, name, factory, version, errorHandler)
        {
        }

        public DBHelper(Context context, string name, int version, SQLiteDatabase.OpenParams openParams) : base(context, name, version, openParams)
        {
        }

        /// <summary>
        /// 第一次创建数据库时调用 在这方法里面可以进行建表
        /// </summary>
        public override void OnCreate(SQLiteDatabase db)
        {
            // 通过execSQL（）执行SQL语句（此处创建了1个名为person的表）
            string sql = @"CREATE TABLE `chat` (
                `id` integer NOT NULL AUTO_INCREMENT,
                `sendusername` varchar(50) NOT NULL DEFAULT '',
                `receiveusername` varchar(50) NOT NULL DEFAULT '',
                `messagetype` integer NOT NULL DEFAULT '0',
                `message` varchar(1024) NOT NULL DEFAULT '',
                `time` bigint(20) NOT NULL DEFAULT '0',
                PRIMARY KEY (`id`),
                KEY `sendusername` (`sendusername`),
                KEY `receiveusername` (`receiveusername`),
                CONSTRAINT `chat_ibfk_1` FOREIGN KEY (`sendusername`) REFERENCES `user` (`username`),
                CONSTRAINT `chat_ibfk_2` FOREIGN KEY (`receiveusername`) REFERENCES `user` (`username`)
            )";
            
            db.ExecSQL(sql);

            // 注：数据库实际上是没被创建 / 打开的（因该方法还没调用）
            // 直到getWritableDatabase() / getReadableDatabase() 第一次被调用时才会进行创建 / 打开 
        }

        /// <summary>
        /// 版本更新的时候调用
        /// </summary>
        public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
        {
            
        }
    }
}