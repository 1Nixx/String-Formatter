using Core;

namespace Tests
{
	public class Tests
	{
		class User
		{
			public string FirstName { get; }
			public string LastName { get; }
			public int Orders { get; }

			public User(string firstName, string lastName, int orders)
			{
				FirstName = firstName;
				LastName = lastName;
				Orders = orders;
			}

			public string GetGreeting()
			{
				return StringFormatter.Shared.Format(
					"Привет, {FirstName} {LastName}!", this);
			}

			public string GetOrderString()
			{
				return StringFormatter.Shared.Format(
					"{FirstName} {LastName} заказал(а) {Orders}", this);
			}

		}

		class TestA
		{
			private int size;
			public bool isSize = false;
			public string StrSize { get; set; }
			

			public TestA(int size)
			{
				this.size = size;

			}

			public void SetStrSize()
			{
				StrSize = StringFormatter.Shared.Format("{{size}}  {isSize}", this);
			}
		}

		[Test]
		public void PublicStringTest()
		{
			var user = new User("Петя", "Иванов", 1);

			var fullName = user.GetOrderString();

			Assert.That(fullName, Is.EqualTo("Петя Иванов заказал(а) 1"));
		}


		[Test]
		public void PublicPropertyAccessTest()
		{
			var user = new User("Петя", "Иванов", 2);

			var result = user.GetGreeting();

			Assert.That(result, Is.EqualTo("Привет, Петя Иванов!"));
		}

		[Test]
		public void FieldAccessTest()
		{
			var test = new TestA(10);

			string result = StringFormatter.Shared.Format("size is {size}", test);

			Assert.That(result, Is.EqualTo("size is 10"));
		}

		[Test]
		public void InvalidSyntaxTest1()
		{
			var test = new TestA(10);

			Assert.Throws<FormatException>(() => StringFormatter.Shared.Format("size is {{size} or {size}}", test));
		}

		[Test]
		public void InvalidSyntaxTest2()
		{
			var test = new TestA(10);

			Assert.Throws<FormatException>(() => StringFormatter.Shared.Format("size is {{size} ", test));
		}

		[Test]
		public void InvalidSyntaxTest3()
		{
			var test = new TestA(10);

			Assert.Throws<FormatException>(() => StringFormatter.Shared.Format("size {is {size}}", test));
		}

		[Test]
		public void CommentTest()
		{
			var test = new TestA(10);

			var result = StringFormatter.Shared.Format("{{size}}  {size}", test);

			Assert.That(result, Is.EqualTo("{size}  10"));
		}

		[Test]
		public void MultiThreadTest()
		{
			var test = new TestA(10);
			var test2 = new TestA(15) { isSize = true };
			string a = StringFormatter.Shared.Format("{{size}}  {size}", test);
			string thread1Str = "";
			string thread2Str = "";

			var thread1 = new Thread(() =>
			{
				thread1Str = StringFormatter.Shared.Format("{{size}}  {size}", test);

				test.SetStrSize();
			});

			var thread2 = new Thread(() =>
			{
				thread2Str = StringFormatter.Shared.Format("{{size}}  {size}", test2);

				test2.SetStrSize();
			});

			thread1.Start();
			thread2.Start();
			thread1.Join();
			thread2.Join();
			Assert.Multiple(() =>
			{
				Assert.That(a, Is.EqualTo("{size}  10"));
				Assert.That(thread1Str, Is.EqualTo("{size}  10"));
				Assert.That(thread2Str, Is.EqualTo("{size}  15"));
				Assert.That(test.StrSize, Is.EqualTo("{size}  False"));
				Assert.That(test2.StrSize, Is.EqualTo("{size}  True"));
			});
		}
	}
}