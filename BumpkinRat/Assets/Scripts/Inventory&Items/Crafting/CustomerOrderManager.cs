using System.Collections.Generic;

namespace BumpkinRat.Crafting
{
    public class CustomerOrderManager 
    {
        private readonly Queue<CustomerOrder> activeOrders;

        private readonly RewardProvisioner rewardProvisioner;

        private static CustomerOrderManager activeOrderManager;

        public static string ActiveOrderDetails => activeOrderManager.GetActiveOrderPromptDetails() ?? string.Empty;

        public CustomerOrderManager()
        {
            this.activeOrders = new Queue<CustomerOrder>();
            this.rewardProvisioner = new RewardProvisioner();

            activeOrderManager = this;
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

        public void SpawnCustomersWithOrders(string queueHeadName, LevelData levelData)
        {
            var orders = this.CreateCustomerOrders(levelData.OrdersInLevel);
            this.SpawnCustomersAtQueueHead(queueHeadName, orders);
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
        private CustomerOrder[] CreateCustomerOrders(OrderDetails[] details)
        {
            CustomerOrder[] orders = new CustomerOrder[details.Length];

            for (int i = 0; i < orders.Length; i++)
            {
                var order = new CustomerOrder(details[i]);
                order.InitializeCustomerData(LevelDataHelper.ActiveLevel);

                orders[i] = order;
            }

            return orders;
        }


        private void SpawnCustomersAtQueueHead(string queueHead = "", params CustomerOrder[] orders) 
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

        private string GetActiveOrderPromptDetails()
        {
            if (!activeOrders.CollectionIsNotNullOrEmpty())
            {
                return null;
            }
            OrderDetails deets = activeOrders.Peek().OrderDetails;
            return deets.GetPromptDisplay();
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