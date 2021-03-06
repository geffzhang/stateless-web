﻿namespace Stateless.Web
{
    using System.IO;
    using LiteDB;

    public class LiteDBStateMachineContentStorage : IStateMachineContentStorage
    {
        // https://github.com/mbdavid/LiteDB/wiki/FileStorage
        private readonly string connectionString;

        public LiteDBStateMachineContentStorage(string connectionString = null)
        {
            this.connectionString = connectionString ?? "stateless.db";
        }

        public Stream Load(StateMachineContext context, string key, Stream stream)
        {
            using var db = new LiteDatabase(this.connectionString);
            if (db.FileStorage.Exists($"{context.Id}/{key}"))
            {
                var fileInfo = db.FileStorage.Download($"{context.Id}/{key}", stream);
            }

            return stream;
        }

        public void Save(StateMachineContext context, string key, Stream stream, string contentType)
        {
            using (var db = new LiteDatabase(this.connectionString))
            {
                if (db.FileStorage.Exists($"{context.Id}/{key}"))
                {
                    db.FileStorage.Delete($"{context.Id}/{key}");
                }

                db.FileStorage.Upload($"{context.Id}/{key}", key, stream);
            }
        }
    }
}
