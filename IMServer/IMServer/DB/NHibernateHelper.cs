using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System;

namespace IMServer.DB
{
    public class NHibernateHelper
    {
        private static ISessionFactory mSessionFactory;//单例模式
        private static readonly object mLock = new object();
        private const string mServerIP = "127.0.0.1";
        private const string mDatabase = "imserver";
        private const string mUsername = "root";
        private const string mPassword = "root";

        private static void InitializeSessionFactory()
        {
            mSessionFactory =
                Fluently.Configure().Database(MySQLConfiguration.Standard
                    .ConnectionString(db => db.Server(mServerIP).Database(mDatabase).Username(mUsername).Password(mPassword)))
                    .Mappings(x => x.FluentMappings.AddFromAssemblyOf<NHibernateHelper>())
                    .BuildSessionFactory();
        }

        private static ISessionFactory SessionFactory
        {
            get
            {
                if (mSessionFactory == null)
                {
                    lock (mLock)
                    {
                        InitializeSessionFactory();
                    }
                }
                return mSessionFactory;
            }
        }

        public static ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }

        public static T OpenDB<T>(Func<ISession, T> func)
        {
            if (func == null) return default(T);
            using (var session = OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    var res = func(session);
                    transaction.Commit();
                    return res;
                }
            }
        }

        public static void OpenDB(Action<ISession> action)
        {
            if (action == null) return;
            using (var session = OpenSession())
            {
                using (var transaction = session.BeginTransaction())
                {
                    action(session);
                    transaction.Commit();
                }
            }
        }
    }
}
