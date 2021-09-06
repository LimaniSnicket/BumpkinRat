using System.Collections.Generic;

namespace BumpkinRat.Crafting
{
    public class CustomerOrderManager 
    {
        private readonly ILevel activeLevel;

        private readonly Queue<CustomerOrder> activeOrders;

        private readonly RewardProvisioner rewardProvisioner;

        private static CustomerOrderManager activeOrderManager;

        public static string ActiveOrderDetails => activeOrderManager.GetActiveOrderPromptDetails() ?? string.Empty;

        public CustomerOrderManager(ILevel level)
        {
            this.activeLevel = level;
            this.activeOrders = new Queue<CustomerOrder>();
            this.rewardProvisioner = new RewardProvisioner();

            if (LevelData.IsActiveLevel(level))
            {
                activeOrderManager = this;
            }
        }

        public static CustomerOrder GetActiveOrder()
        {
            activeOrderManager.TryGetNextUpOrder(out CustomerOrder active);
            return active;
        }

        public static void EvaluateRecipeBasedOnCustomerOrder(Recipe recipe)
        {
            activeOrderManager.EvaluateAgainstRecipe(recipe);
        }

        public CustomerOrder[] CreateCustomerOrders(params (int, OrderType, int)[] orderParams)
        {
            CustomerOrder[] orders = new CustomerOrder[orderParams.Length];
            int i = 0;

            foreach (var order in orderParams)
            {
                var details = this.CreateCustomerOrder(order.Item2, order.Item3);
                var completeOrder = new CustomerOrder(order.Item1, details);
                completeOrder.InitializeCustomerData(activeLevel);
                orders[i] = completeOrder;
                i++;
            }

            return orders;
        }

        public void SpawnCustomersAtQueueHead(string queueHead = "", params CustomerOrder[] orders) 
        {
            bool validQueuePosition = CustomerQueueHeadManager.TryGetCustomerQueueHead(queueHead, out CustomerQueueHead head);

            if (validQueuePosition)
            {
                for (int i = 0; i < orders.Length; i++)
                {
                    activeOrders.Enqueue(orders[i]);

                    CustomerNpc customer = CustomerNpcSpawner.GetCustomerNpcPrefab();
                    customer.SetCustomerAppearence(orders[i].NpcId);

                    head.EnqueueCustomers(customer);
                }
            }
        }

        public void EvaluateAgainstRecipe(Recipe r)
        {
            if (TryGetNextUpOrder(out CustomerOrder order))
            {
                if (order.CompareTo(r) == 0)
                {
                    CompleteActiveOrder(order);
                }
            }
        }

        private OrderDetails CreateCustomerOrder(OrderType orderType, int orderId)
        {
            return new OrderDetails
            {
                orderLookupId = orderId,
                orderType = orderType,
                cashReward = 10,
                rewardItemIds = new int[] { 3 },
                orderTitle = "Nunchuck Nightmare!",
                orderPrompt = "<b><color=red>Attach</color></b> the <b><color=red>Broken Links</color></b> to fix it for your cuzzo!"
            };
        }

        private string GetActiveOrderPromptDetails()
        {
            if (!activeOrders.CollectionIsNotNullOrEmpty())
            {
                return null;
            }
            OrderDetails deets = activeOrders.Peek().OrderDetails;
            return deets.DetailsToString();
        }

        private void CompleteActiveOrder(CustomerOrder order)
        {
            rewardProvisioner.DropItemRewards(order.OrderDetails);
            order.CacheCustomerDialogue();
            var completedOrder = activeOrders.Dequeue();
        }

        private bool TryGetNextUpOrder(out CustomerOrder order)
        {
            bool valid = activeOrders.CollectionIsNotNullOrEmpty();
            order = valid ? activeOrders.Peek() : null;
            return valid;
        }
    }
}