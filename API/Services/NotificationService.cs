﻿using API.Entity;
using API.Interface.Repository;
using API.Interface.Service;
using API.Param;
using API.Param.Enums;
using Microsoft.IdentityModel.Tokens;
using Account = API.Entity.Account;

namespace API.Services
{
    public class NotificationService : INotificatonService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IFirebaseMessagingService _messagingService;
        private readonly IAccountRepository _accountRepository;
        private readonly IRealEstateRepository _realEstateRepository;
        private readonly IAuctionRepository _auctionRepository;
        private readonly IAuctionAccountingRepository _auctionAccountingRepository;

        public NotificationService(INotificationRepository notificationRepository, IFirebaseMessagingService messagingService, IAccountRepository accountRepository, IRealEstateRepository realEstateRepository, IAuctionRepository auctionRepository)
        {
            _notificationRepository = notificationRepository;
            _messagingService = messagingService;
            _accountRepository = accountRepository;
            _realEstateRepository = realEstateRepository;
            _auctionRepository = auctionRepository;
        }

        public async Task<List<Notification>> GetNotificationsOrderByDateCreate(int accountId)
        {
            var notificationList = await _notificationRepository.GetNotificationsBaseOnAccountId(accountId);
            var orderNotificationList = notificationList.OrderByDescending(n => n.DateCreated).ToList();

            return orderNotificationList;
        }


        public async System.Threading.Tasks.Task SendNotificationWhenMemberCreateReal(RealEstate realEstate)
        {
            List<Account> staffAndAdminAccount = await _accountRepository.GetAllStaffAndAdminAccounts();
            Account realEstateOwnerAccount = await _accountRepository.GetAccountByAccountIdAsync(realEstate.AccountOwnerId);

            string title = "New real estate posted!";
            string body = $"New real estate with name of {realEstate.ReasName} has been created by {realEstateOwnerAccount.AccountName} at {realEstate.DateCreated.ToString("dd/MM/yyyy HH:mm")}";

            int type = (int)NotificationTypeEnum.NewRealEstateCreate;
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            foreach (var account in staffAndAdminAccount)
            {
                notification.AccountReceiveId = account.AccountId;
                notification.NotificationId = 0;
                await _notificationRepository.CreateAsync(notification);

                if (!account.FirebaseToken.IsNullOrEmpty())
                {
                    await _messagingService.SendPushNotification(account.FirebaseToken, title, body, data);
                }
            }
        }

        public async System.Threading.Tasks.Task SendNotificationWhenApproveRealEstate(ReasStatusParam reasStatusParam, RealEstate realEstate)
        {
            //defualt is denied
            string title = "Your real estate has been denied";
            string body = $"Your real estate at {realEstate.ReasAddress} does not meet our requirements. Please check and make sure you upload all the required paper work. ";
            int type = (int)NotificationTypeEnum.NewRealEstateRejected;

            if (reasStatusParam.reasStatus == (int)RealEstateStatus.Approved)
            {
                title = "Your real estate has been approved";
                body = $"Congratulation, your real estate {realEstate.ReasName} at {realEstate.ReasAddress} has been approved. To continue with the process, please pay the uploading fee";
                type = (int)NotificationTypeEnum.NewRealEstateApproved;

            }
            else if (!reasStatusParam.messageString.IsNullOrEmpty())
            {
                //case not approve and has message from admin
                body = body + reasStatusParam.messageString;
            }

            Account ownerAccount = await _accountRepository.GetAccountByAccountIdAsync(realEstate.AccountOwnerId);
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
                AccountReceiveId = ownerAccount.AccountId
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            if (!ownerAccount.FirebaseToken.IsNullOrEmpty())
            {
                await _messagingService.SendPushNotification(ownerAccount.FirebaseToken, title, body, data);
            }

            await _notificationRepository.CreateAsync(notification);

        }

        public async System.Threading.Tasks.Task SendNotificationWhenCreateAuction(int auctionId)
        {
            Auction auction = _auctionRepository.GetAuction(auctionId);
            RealEstate realEstate = _realEstateRepository.GetRealEstate(auction.ReasId);
            string title = $"Your deposited real estate at {realEstate.ReasAddress} now have an auction date";
            string body = $"Your upcoming auction is scheduled for {realEstate.ReasName} on {auction.DateStart.ToString("\"dd/MM/yyyy HH:mm\"")}. Please be prepared.";

            int type = (int)NotificationTypeEnum.NewAuctionCreate;
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            List<Account> userLists = await _auctionRepository.GetAccoutnDepositedInAuctionUsingReasId(auction.ReasId);

            foreach (var account in userLists)
            {
                notification.AccountReceiveId = account.AccountId;
                notification.NotificationId = 0;
                await _notificationRepository.CreateAsync(notification);

                if (!account.FirebaseToken.IsNullOrEmpty())
                {
                    await _messagingService.SendPushNotification(account.FirebaseToken, title, body, data);
                }
            }
        }

        public async System.Threading.Tasks.Task SendNotificationWhenAuctionAboutToStart(int auctionId)
        {
            Auction auction = _auctionRepository.GetAuction(auctionId);
            RealEstate realEstate = _realEstateRepository.GetRealEstate(auction.ReasId);
            string title = "Auction about to start!";
            string body = $"Your upcoming auction is scheduled for {realEstate.ReasName} on {auction.DateStart.ToString("\"dd/MM/yyyy HH:mm\"")}. Please be prepared.";

            int type = (int)NotificationTypeEnum.AuctionAboutToStart;
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            List<Account> userLists = await _auctionRepository.GetAccoutnDepositedInAuctionUsingReasId(auction.ReasId);

            foreach (var account in userLists)
            {
                notification.AccountReceiveId = account.AccountId;
                notification.NotificationId = 0;
                await _notificationRepository.CreateAsync(notification);

                if (!account.FirebaseToken.IsNullOrEmpty())
                {
                    await _messagingService.SendPushNotification(account.FirebaseToken, title, body, data);
                }
            }
        }

        public async System.Threading.Tasks.Task SendNotificationWhenWinAuction(int auctionId)
        {
            AuctionAccounting auctionAccounting = _auctionAccountingRepository.GetAuctionAccountingByAuctionId(auctionId);
            Account winnerAccount = await _accountRepository.GetAccountOnId(auctionAccounting.AccountWinId);
            RealEstate realEstate = _realEstateRepository.GetRealEstate(auctionAccounting.ReasId);


            string title = "Congratuation, you have won the auction!";
            string body = $"You are the highest bidder for real estate at {realEstate.ReasAddress}. Congratuation, Our staff will contact you for futher instructions";

            int type = (int)NotificationTypeEnum.AuctionFinishWinner;
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
                AccountReceiveId = winnerAccount.AccountId
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            await _notificationRepository.CreateAsync(notification);

            if (!winnerAccount.FirebaseToken.IsNullOrEmpty())
            {
                await _messagingService.SendPushNotification(winnerAccount.FirebaseToken, title, body, data);
            }

        }



        public async System.Threading.Tasks.Task SendNotificationWhenLoseAuction(List<int> accountIdParticipateInAuction, int auctionId)
        {
            if (accountIdParticipateInAuction.Count > 0)
            {
                List<Account> accounts = new List<Account>();
                foreach (int id in accountIdParticipateInAuction)
                {
                    var account = await _accountRepository.GetAccountByAccountIdAsync(id);
                    if (account != null)
                    {
                        accounts.Add(account);
                    }
                }

                SendNotificationWhenNotWinAuction(accounts, auctionId, false);

            }

        }

        public async System.Threading.Tasks.Task SendNotificationWhenNotAttendAuction(List<int> accountIdParticipateInAuction, int auctionId)
        {
            if (accountIdParticipateInAuction.Count > 0)
            {
                List<Account> accounts = new List<Account>();
                foreach (int id in accountIdParticipateInAuction)
                {
                    var account = await _accountRepository.GetAccountByAccountIdAsync(id);
                    if (account != null)
                    {
                        accounts.Add(account);
                    }
                }

                SendNotificationWhenNotWinAuction(accounts, auctionId, true);

            }

        }


        public async System.Threading.Tasks.Task SendNotificationToStaffandAdminWhenAuctionFinish(int auctionId)
        {
            AuctionAccounting auctionAccounting = _auctionAccountingRepository.GetAuctionAccountingByAuctionId(auctionId);
            RealEstate realEstate = _realEstateRepository.GetRealEstate(auctionAccounting.ReasId);

            List<Account> staffAndAdminAccount = await _accountRepository.GetAllStaffAndAdminAccounts();

            string title = "Auction for realestate has completed";
            string body = $"The auction on real estate as {realEstate.ReasAddress} has finished!. Please check the auction list for futher detail";

            int type = (int)NotificationTypeEnum.AuctionFinishAdminAndStaff;
            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            foreach (var account in staffAndAdminAccount)
            {
                notification.AccountReceiveId = account.AccountId;
                notification.NotificationId = 0;
                await _notificationRepository.CreateAsync(notification);

                if (!account.FirebaseToken.IsNullOrEmpty())
                {
                    await _messagingService.SendPushNotification(account.FirebaseToken, title, body, data);
                }
            }
        }



        public async System.Threading.Tasks.Task SendNotificationWhenNotWinAuction(List<Account> accounts, int auctionId, bool isNotAttendAuction)
        {
            AuctionAccounting auctionAccounting = _auctionAccountingRepository.GetAuctionAccountingByAuctionId(auctionId);
            RealEstate realEstate = _realEstateRepository.GetRealEstate(auctionAccounting.ReasId);

            string title = "Auction Bid Confirmation: You Lost";
            string body = $"The auction on real estate as {realEstate.ReasAddress} has finished!. The real estate has been bought with {auctionAccounting.MaxAmount}VND. Our staff will contact you for the refund process";

            int type = (int)NotificationTypeEnum.AuctionFinishLoser;

            if (isNotAttendAuction)
            {
                body = $"You did not attend the auction for the real estate at {realEstate.ReasAddress}, and as a result, you have lost your deposit. If you have any questions, please contact our staff for assistance.";
                type = (int)NotificationTypeEnum.AuctionFinishNotAttender;
            }

            Notification notification = new Notification
            {
                NotificationType = type,
                Title = title,
                Body = body,
                DateCreated = DateTime.Now,
            };

            Dictionary<string, string> data = new Dictionary<string, string>
            {
                { "type", type.ToString() }
            };

            foreach (var account in accounts)
            {
                notification.AccountReceiveId = account.AccountId;
                notification.NotificationId = 0;
                await _notificationRepository.CreateAsync(notification);

                if (!account.FirebaseToken.IsNullOrEmpty())
                {
                    await _messagingService.SendPushNotification(account.FirebaseToken, title, body, data);
                }
            }
        }
    }


}