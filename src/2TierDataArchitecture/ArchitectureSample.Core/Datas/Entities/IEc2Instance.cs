using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.EC2;
using Amazon.EC2.Model;

namespace ArchitectureSample.Core.Datas.Entities
{
    public interface IEc2Instance
    {
        bool Success { get; set; }
        string InstanceId { get; set; }
        string SpotInstanceRequestId { get; set; }
        string IamInstanceProfile { get; set; }
        string ImageId { get; set; }
        string State { get; set; }
        bool IsSpot { get; set; }
        DateTime LaunchTime { get; set; }
        string Platform { get; set; }
        string PrivateIpAddress { get; set; }
        string PublicDnsName { get; set; }
        string[] SecurityGroupIds { get; set; }
        string Architecture { get; set; }
        string SubnetId { get; set; }
        string VpcId { get; set; }
        Dictionary<string, string> Tags { get; set; }
        string EndPoint { get; }
    }

    public class Ec2Instance : IEc2Instance
    {
        public bool Success { get; set; }
        public string InstanceId { get; set; }
        public string SpotInstanceRequestId { get; set; }
        public string IamInstanceProfile { get; set; }
        public string ImageId { get; set; }
        public string State { get; set; }
        public bool IsSpot { get; set; }
        public DateTime LaunchTime { get; set; }
        public string Platform { get; set; }
        public string PrivateIpAddress { get; set; }
        public string PublicDnsName { get; set; }
        public string[] SecurityGroupIds { get; set; }
        public string Architecture { get; set; }
        public string SubnetId { get; set; }
        public string VpcId { get; set; }
        public Dictionary<string, string> Tags { get; set; }
        public string EndPoint => Env.IsDebug ? PublicDnsName : PrivateIpAddress;

        public Ec2Instance()
        {

        }

        public IEc2Instance Bind(bool success, Instance x)
        {
            Success = success;
            InstanceId = x.InstanceId;
            SpotInstanceRequestId = x.SpotInstanceRequestId;
            IamInstanceProfile = x.IamInstanceProfile?.Arn;
            ImageId = x.ImageId;
            State = x.State?.Name?.Value;
            IsSpot = x.InstanceLifecycle == InstanceLifecycleType.Spot;
            LaunchTime = x.LaunchTime;
            PrivateIpAddress = x.PrivateIpAddress;
            PublicDnsName = x.PublicDnsName;
            SecurityGroupIds = x.SecurityGroups.Select(y => y.GroupId).ToArray();
            Architecture = x.Architecture.Value;
            SubnetId = x.SubnetId;
            VpcId = x.VpcId;
            Tags = x.Tags.ToDictionary(kv => kv.Key, kv => kv.Value);

            return this;
        }

        public IEc2Instance Bind(bool success, IEc2Instance x)
        {
            Success = success;
            InstanceId = x.InstanceId;
            SpotInstanceRequestId = x.SpotInstanceRequestId;
            IamInstanceProfile = x.IamInstanceProfile;
            ImageId = x.ImageId;
            IsSpot = x.IsSpot;
            State = x.State;
            LaunchTime = x.LaunchTime;
            PrivateIpAddress = x.PrivateIpAddress;
            PublicDnsName = x.PublicDnsName;
            SecurityGroupIds = x.SecurityGroupIds;
            Architecture = x.Architecture;
            SubnetId = x.SubnetId;
            VpcId = x.VpcId;
            Tags = x.Tags;

            return this;
        }
    }

    public static class IEC2InstanceExtensions
    {
        public static bool IsTagExists(this IEc2Instance source, string key, string value)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Tags.Where(x => x.Key == key).Where(x => x.Value == value).Any();
        }

        public static bool IsTagExists(this IEc2Instance source, string key, Func<string, bool> valueFunc)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            return source.Tags.Where(x => x.Key == key).Where(x => valueFunc.Invoke(x.Value)).Any();
        }
    }
}
