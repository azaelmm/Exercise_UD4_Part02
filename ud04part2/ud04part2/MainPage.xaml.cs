using Firebase.Database;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Formats.Tar;
using Firebase.Database.Query;

namespace ud04part2
{
    public partial class MainPage : ContentPage
    {
        private const string FirebaseUrl = "https://actfirebase317-default-rtdb.europe-west1.firebasedatabase.app/";
        private readonly FirebaseClient firebaseClient;

        public ObservableCollection<TaskItem> Tasks { get; set; }

        public MainPage()
        {
            InitializeComponent();

            firebaseClient = new FirebaseClient(FirebaseUrl);
            Tasks = new ObservableCollection<TaskItem>();
            BindingContext = this;

            LoadTasks();
        }

        private async void LoadTasks()
        {
            var firebaseTasks = await firebaseClient.Child("Tareas").OnceAsync<TaskItem>();
            Tasks.Clear();

            foreach (var item in firebaseTasks)
            {
                Tasks.Add(new TaskItem
                {
                    Id = item.Key,
                    NombreTarea = item.Object.NombreTarea
                });
            }
        }

        private async void AddTask(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(TaskEntry.Text))
            {
                var newTask = new TaskItem { NombreTarea = TaskEntry.Text };
                var response = await firebaseClient.Child("Tareas").PostAsync(newTask);

                newTask.Id = response.Key;
                Tasks.Add(newTask);
                TaskEntry.Text = string.Empty;
            }
        }

        private async void UpdateTask(object sender, EventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem selectedTask && !string.IsNullOrEmpty(TaskEntry.Text))
            {
                selectedTask.NombreTarea = TaskEntry.Text;
                await firebaseClient.Child("Tareas").Child(selectedTask.Id).PutAsync(selectedTask);

                LoadTasks();
                TaskEntry.Text = string.Empty;
            }
        }

        private async void DeleteTask(object sender, EventArgs e)
        {
            if (TaskList.SelectedItem is TaskItem selectedTask)
            {
                await firebaseClient.Child("Tareas").Child(selectedTask.Id).DeleteAsync();
                Tasks.Remove(selectedTask);
            }
        }

        private void OnTaskSelected(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is TaskItem selectedTask)
            {
                TaskEntry.Text = selectedTask.NombreTarea;
            }
        }

    }

    public class TaskItem
    {
        public string Id { get; set; }
        public string NombreTarea { get; set; }
    }

}
