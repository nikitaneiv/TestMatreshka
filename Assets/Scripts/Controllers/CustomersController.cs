using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using CookingPrototype.Kitchen;

namespace CookingPrototype.Controllers 
{
	public class CustomersController : MonoBehaviour 
	{
		public static CustomersController Instance { get; private set; }

		public int CustomersTargetNumber = 15;
		public float CustomerWaitTime = 18f;
		public float CustomerSpawnTime = 3f;
		public List<CustomerPlace> CustomerPlaces = null;

		[HideInInspector] public int TotalCustomersGenerated { get; private set; } = 0;

		public event Action TotalCustomersGeneratedChanged;

		const string CUSTOMER_PREFABS_PATH = "Prefabs/Customer";

		float _timer = 0f;
		Stack<List<Order>> _orderSets;

		bool HasFreePlaces 
		{
			get { return CustomerPlaces.Any(x => x.IsFree); }
		}

		public bool IsComplete 
		{
			get 
			{
				return TotalCustomersGenerated >= CustomersTargetNumber && CustomerPlaces.All(x => x.IsFree);
			}
		}

		void Awake() 
		{
			if ( Instance != null ) 
			{
				Debug.LogError("Another instance of CustomersController already exists!");
			}

			Instance = this;
		}

		void OnDestroy() 
		{
			if ( Instance == this ) 
			{
				Instance = null;
			}
		}

		void Start() 
		{
			Init();
		}

		void Update() 
		{
			if ( !HasFreePlaces ) 
			{
				return;
			}

			_timer += Time.deltaTime;

			if ( (TotalCustomersGenerated >= CustomersTargetNumber) || (!(_timer > CustomerSpawnTime)) ) 
			{
				return;
			}

			SpawnCustomer();
			_timer = 0f;
		}

		void SpawnCustomer() 
		{
			var freePlaces = CustomerPlaces.FindAll(x => x.IsFree);
			if ( freePlaces.Count <= 0 ) 
			{
				return;
			}

			var place = freePlaces[Random.Range(0, freePlaces.Count)];
			place.PlaceCustomer(GenerateCustomer());
			TotalCustomersGenerated++;
			TotalCustomersGeneratedChanged?.Invoke();
		}

		Customer GenerateCustomer() 
		{
			var customerGo = Instantiate(Resources.Load<GameObject>(CUSTOMER_PREFABS_PATH));
			var customer = customerGo.GetComponent<Customer>();

			var orders = _orderSets.Pop();
			customer.Init(orders);

			return customer;
		}

		Order GenerateRandomOrder() 
		{
			var oc = OrdersController.Instance;
			return oc.Orders[Random.Range(0, oc.Orders.Count)];
		}

		public void Init() 
		{
			var totalOrders = 0;
			_orderSets = new Stack<List<Order>>();
			for ( var i = 0; i < CustomersTargetNumber; i++ ) {
				var orders = new List<Order>();
				var ordersNum = Random.Range(1, 4);
				for ( var j = 0; j < ordersNum; j++ ) {
					orders.Add(GenerateRandomOrder());
				}

				_orderSets.Push(orders);
				totalOrders += ordersNum;
			}

			CustomerPlaces.ForEach(x => x.Free());
			_timer = 0f;

			TotalCustomersGenerated = 0;
			TotalCustomersGeneratedChanged?.Invoke();

			GameplayController.Instance.OrdersTarget = totalOrders - 2;
		}

		/// <summary>
		/// Отпускаем указанного посетителя
		/// </summary>
		/// <param name="customer"></param>
		
		
		public void FreeCustomer(Customer customer) 
		{
			var place = CustomerPlaces.Find(x => x.CurCustomer == customer);
			if ( place == null ) 
			{
				return;
			}

			place.Free();
			GameplayController.Instance.CheckGameFinish();
		}

		/// <summary>
		///  Пытаемся обслужить посетителя с заданным заказом и наименьшим оставшимся временем ожидания.
		///  Если у посетителя это последний оставшийся заказ из списка, то отпускаем его.
		/// </summary>
		/// <param name="order">Заказ, который пытаемся отдать</param>
		/// <returns>Флаг - результат, удалось ли успешно отдать заказ</returns>

		public bool ServeOrder(Order order) {
			//throw new NotImplementedException("ServeOrder: this feature is not implemented.");
			
		// 	var customers = CustomerPlaces;
		// 	var customersWithOrder = new List<Customer>();
		// 	foreach (var customerPlace in customers)
		// 	{
		// 		var customer = customerPlace.CurCustomer;
		// 		if (customer == null)
		// 		{
		// 			continue;
		// 		}
		//
		// 		var customerOrders = customer.OrderPlaces.Select(op => op.CurOrder);
		// 		if (customerOrders.Contains(order))
		// 		{
		// 			customersWithOrder.Add(customer);
		// 		}
		// 	}
		//
		// 	if (customersWithOrder.Count == 0)
		// 	{
		// 		return false;
		// 	}
		//
		// 	var customerToServe = customersWithOrder.OrderBy(c => c.WaitTime).First();
		// 	if (customerToServe.ServeOrder(order))
		// 	{
		// 		if (customerToServe.IsComplete)
		// 		{
		// 			FreeCustomer(customerToServe);
		// 		}
		// 		return true;
		// 	}
		//
		// 	return false;
		var customers = CustomerPlaces;
		var customersWithOrder = new List<Customer>();
  
		// Находим всех посетителей, у которых имеется требуемый заказ
		foreach (var customerPlace in customers)
		{
			var customer = customerPlace.CurCustomer;
			if (customer == null)
			{
				continue;
			}

			var customerOrders = customer.OrderPlaces.Select(op => op.CurOrder);
			if (customerOrders.Contains(order))
			{
				customersWithOrder.Add(customer);
			}
		}

		// Если нет посетителей с нужным заказом, возвращаем false
		if (customersWithOrder.Count == 0)
		{
			return false;
		}

		// Ищем посетителя с наименьшим временем ожидания и пытаемся отдать ему заказ
		var customerToServe = customersWithOrder.OrderBy(c => c.WaitTime).First();
		if (customerToServe.ServeOrder(order))
		{
			// Если у посетителя закончились заказы, он освобождается
			if (customerToServe.IsComplete)
			{
				FreeCustomer(customerToServe);
			}
			return true;
		}

		// Если не удалось отдать заказ, возвращаем false
		return false;
		}
		}
}