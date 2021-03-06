﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;

///注意项目的.net Framework版本是4.6.1
///MongoDB.Driver版本是2.5.0
///PM>Install-Package MongoDB.Driver -Version 2.5.0
///具体的使用版本是可以在package.config中查看的
///若是在安装MongoDB.Driver时使用默认的最新版会出现一些问题

///MongoDB 默认用 id 做主键，因此不用显式指定 id 是主键
///对象的Id属性在MongoDB中默认存为“_id”
///MongoDB 中没有内置“自增字段”，可以把 Id 声明为 ObjectId 类型（using MongoDB.Bson）这样插入以后就自动给字段赋值。

///MongoDB的使用场景：
///注意所有的NoSql数据库都不是为了取代关系型数据库，也没没有可能取代关系型数据库，
///MongoDB使用场景，主要是许多数据他的字段是不一样的，但是他们有需要在一个表中
///举一个例子，商场的一张商品表，存放所有的商品，但是食品需要保存保质期的信息，而衣服不需要保质期的信息，而需要颜色和型号的信息
///此时你把所有的商品放在一张表中，表的字段设置就有一些麻烦了，但是你使用MongoDB中的Collection就毫无问题
///对于爬虫你爬取的信息可能存放在一张表中就比较麻烦，但是存在Collection中就很简单
///但是对于一些关键性的数据比如订单等涉及到金钱的信息尽量就不要使用MongoDB
///
///同时之前对经纬度的操作查询都是使用Redis中的geo类型的数据，MongoDB中也是可是存储经纬度，并对其操作的。
///
///有这么一种说法（未经考证）：
///因为MongoDB时文件型数据库，而Redis是键值型数据库，
///所以MongoDB要比Redis复杂的多，因此作为一个架构师你在作技术选型的时候，能用Redis就不要使用MongoDB
///
namespace _016MongoDBDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //创建MongoDB数据库对象，插入数据
            //InsertOneInMongoDB();


            //插入Josn类型的数据
            //InsertJosnInMongoDB();

            //插入潜逃数据
            InsertNestedObject();

            Console.WriteLine("OK");
            Console.ReadKey();
        }

        //创建MongoDB数据库对象，插入数据
        static void InsertOneInMongoDB()
        {
            //连接MongoDB服务，创建对象
            MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
            //获取名为：TestDb1的数据库，若是没有则创建！
            IMongoDatabase db = client.GetDatabase("TestDb1");
            //获取名为名为Personsde表（collection可以理解为表）若是没有则创建！
            IMongoCollection<Person> persons = db.GetCollection<Person>("Persons");
            IMongoCollection<Dog> dogs = db.GetCollection<Dog>("Dogs");

            Person p1 = new Person() { Id = 0001, Name = "shanzm", Age = 25 };
            Person p2 = new Person() { Id = 002, Name = "shanzm" };//MongoDB会对Age默认填充为0

            Dog d1 = new Dog() { Name = "史努比" };//注意因为Dog类的Id是ObjectId类型，所以MongoDB会自动生成一个Id值

            persons.InsertOne(p1);
            persons.InsertOne(p2);

            dogs.InsertOne(d1);
        }

        //插入Josn类型的数据
        static void InsertJosnInMongoDB()
        {
            MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = client.GetDatabase("TestDb1");
            IMongoCollection<BsonDocument> dogs = db.GetCollection<BsonDocument>("Dogs");

            string json = "{Name:'大黄',Age:10,Weight:50}";
            BsonDocument d1 = BsonDocument.Parse(json);//解析Json数据
            dogs.InsertOne(d1);

            //这里其实是可以对比上面把Dog对象插入MongoDB中，
            //我们在这里的Josn数据的属性要比Dog对象的属性多，但是依旧可以插入到Dogs表中
            //这也就是表明MongoDB数据库中的表是没有结构的（你插入的每一条数据都是以“文件”存储的）

            //虽然存放数据的时候对每一个属性的类型和是否有值不做要求，
            //但是你要是想统一读取数据，那么MongoDB中的Collection中的数据格式要保持统一
        }

        //插入有嵌套关系的对象，比如小狗和主人，
        //注意不会有表间关系，都是存到一个集合中，注意和关系库不一样。
        static void InsertNestedObject()
        {
            MongoClient client = new MongoClient("mongodb://127.0.0.1:27017");
            IMongoDatabase db = client.GetDatabase("TestDb1");
            IMongoCollection<Dog> dogs = db.GetCollection<Dog>("Dogs");

            Dog d = new Dog() { Name = "小白", Master = new Person() { Name = "shanzm", Age = 25 } };
            dogs.InsertOne(d);
        }

    }
}
