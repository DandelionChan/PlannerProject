// ------------------------
// Modal Selectors
// ------------------------
const eventModal = document.getElementById("eventModal");
const openEventBtn = document.getElementById("openEventModal");
const closeEventBtn = document.getElementById("closeModal");

const reminderModal = document.getElementById("reminderModal");
const openReminderBtn = document.getElementById("openReminderModal");
const closeReminderBtn = document.getElementById("closeReminderModal");

// ------------------------
// Toggle Logic
// ------------------------
openEventBtn.onclick = () => eventModal.classList.remove("hidden");
closeEventBtn.onclick = () => eventModal.classList.add("hidden");

openReminderBtn.onclick = () => {
    reminderModal.classList.remove("hidden");
    loadReminders();
};
closeReminderBtn.onclick = () => reminderModal.classList.add("hidden");

window.onclick = (e) => {
    if (e.target === eventModal) eventModal.classList.add("hidden");
    if (e.target === reminderModal) reminderModal.classList.add("hidden");
};

// ------------------------
// Reminder Specific Logic
// ------------------------
async function loadReminders() {
    try {
        const response = await fetch('/DailyReminders/GetAllForManagement');
        const data = await response.json();

        let html = '<div style="margin-top: 15px;">';
        data.forEach(rem => {
            html += `
                <div style="display: flex; justify-content: space-between; align-items: center; padding: 12px; border-bottom: 1px solid #eee;">
                    <span style="font-size: 14px; font-weight: 500;">${rem.text}</span>
                    <input type="checkbox" style="width: 20px; height: 20px; cursor: pointer;" 
                           ${rem.isActive ? 'checked' : ''} 
                           onchange="toggleReminder(${rem.id}, this.checked)">
                </div>`;
        });
        html += '</div>';
        document.getElementById('reminderListContainer').innerHTML = html;
    } catch (err) {
        document.getElementById('reminderListContainer').innerHTML = "Error loading reminders.";
    }
}

async function toggleReminder(id, isChecked) {
    const url = isChecked ? '/DailyReminders/Subscribe' : '/DailyReminders/Unsubscribe';
    const body = new URLSearchParams();
    body.append('id', id);

    try {
        await fetch(url, { method: 'POST', body: body });
        updateCalendar();
    } catch (err) {
        console.error("Update failed", err);
    }
}

// ------------------------
// Calendar Core Logic
// ------------------------
let currentDate = new Date();

function getStartOfWeek(date) {
    const d = new Date(date);
    const day = d.getDay();
    const diff = d.getDate() - day + (day === 0 ? -6 : 1);
    const start = new Date(d.setDate(diff));
    start.setHours(0, 0, 0, 0);
    return start;
}

function getWeekDates(date) {
    const start = getStartOfWeek(date);
    const week = [];
    for (let i = 0; i < 7; i++) {
        const day = new Date(start);
        day.setDate(start.getDate() + i);
        week.push(day);
    }
    return week;
}

function renderWeek() {
    const weekContainer = document.getElementById("weekDays");
    weekContainer.innerHTML = "";
    const week = getWeekDates(currentDate);
    const today = new Date().toDateString();

    week.forEach((date, index) => {
        const div = document.createElement("div");
        div.classList.add("week-day");
        if (date.toDateString() === today) div.classList.add("today");
        div.innerText = date.toLocaleDateString(undefined, { weekday: "short", day: "numeric", month: "short" });
        weekContainer.appendChild(div);

        const dayMap = [1, 2, 3, 4, 5, 6, 0];
        const column = document.querySelector(`.day-column[data-day="${dayMap[index]}"]`);
        if (column) {
            const localDate = date.getFullYear() + "-" + String(date.getMonth() + 1).padStart(2, '0') + "-" + String(date.getDate()).padStart(2, '0');
            column.dataset.date = localDate;
        }
    });
}

async function loadWeekEvents() {
    const start = getStartOfWeek(currentDate);
    const end = new Date(start);
    end.setDate(start.getDate() + 6);

    const s = start.toISOString().split('T')[0];
    const e = end.toISOString().split('T')[0];

    try {
        const response = await fetch(`/calendar/events?start=${s}&end=${e}`);
        const events = await response.json();
        document.querySelectorAll(".calendar-event").forEach(el => el.remove());
        renderEvents(events);
    } catch (error) {
        console.error("Error loading events:", error);
    }
}

function renderEvents(events) {
    const pixelsPerMinute = 1;
    const groups = {};

    // Clear existing events first
    document.querySelectorAll(".calendar-event").forEach(el => el.remove());

    events.forEach(event => {
        if (event.start_datetime) {
            const dateKey = event.start_datetime.split("T")[0]; // Declare dateKey here
            if (!groups[dateKey]) groups[dateKey] = [];
            groups[dateKey].push(event);
        }
    });

    for (const dateKey in groups) { // Use dateKey consistently
        const column = document.querySelector(`.day-column[data-date="${dateKey}"]`);
        if (!column) continue;

        const dayEvents = groups[dateKey];
        dayEvents.sort((a, b) => new Date(a.start_datetime) - new Date(b.start_datetime));

        dayEvents.forEach((event) => {
            const start = new Date(event.start_datetime);
            const end = event.end_datetime ? new Date(event.end_datetime) : new Date(start.getTime() + 60 * 60 * 1000);

            const collisions = dayEvents.filter(other => {
                const oStart = new Date(other.start_datetime);
                const oEnd = other.end_datetime ? new Date(other.end_datetime) : new Date(oStart.getTime() + 60 * 60 * 1000);
                return (start < oEnd && end > oStart);
            });

            const eventEl = document.createElement("div");
            eventEl.classList.add("calendar-event");
            eventEl.style.backgroundColor = event.color;
            eventEl.innerHTML = `<strong>${event.title || event.name}</strong>`;

            const top = (start.getHours() * 60 + start.getMinutes()) * pixelsPerMinute;
            const height = Math.max(25, (end - start) / (1000 * 60) * pixelsPerMinute);
            const width = 95 / collisions.length;
            const left = (collisions.indexOf(event) * width) + 2;

            eventEl.style.top = `${top}px`;
            eventEl.style.height = `${height}px`;
            eventEl.style.width = `${width}%`;
            eventEl.style.left = `${left}%`;

            column.appendChild(eventEl);
        });
    }
}
// ------------------------
// Event Creation (POST)
// ------------------------
const eventForm = document.getElementById("eventForm");

if (eventForm) {
    eventForm.onsubmit = async (e) => {
        e.preventDefault();

        const formData = new FormData(eventForm);
        const payload = {
            type: formData.get("type"),
            title: formData.get("title"),
            description: formData.get("description"),
            color: formData.get("color"),
            startDate: formData.get("event_date"), // Matches 'event_date' name in HTML
            startTime: formData.get("start_time") || "00:00",
            endTime: formData.get("end_time") || "01:00",
            recurrence: formData.get("recurrence_type")
        };

        try {
            const response = await fetch('/calendar/events', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify(payload)
            });

            const result = await response.json();
            if (result.success) {
                eventModal.classList.add("hidden");
                eventForm.reset();
                updateCalendar(); // Refresh the view immediately
            } else {
                alert("Failed to save event.");
            }
        } catch (err) {
            console.error("Error saving event:", err);
        }
    };
}

// Logic to show/hide time fields based on type
const eventTypeSelect = document.getElementById("eventType");
const timeFields = document.getElementById("timeFields");

if (eventTypeSelect && timeFields) {
    eventTypeSelect.onchange = (e) => {
        if (e.target.value === "appointment") {
            timeFields.classList.remove("hidden");
        } else {
            timeFields.classList.add("hidden");
        }
    };
}

function updateCalendar() {
    renderWeek();
    loadWeekEvents();
}

// Navigation
document.getElementById("prevWeek").onclick = () => { currentDate.setDate(currentDate.getDate() - 7); updateCalendar(); };
document.getElementById("nextWeek").onclick = () => { currentDate.setDate(currentDate.getDate() + 7); updateCalendar(); };

// Initial Boot
updateCalendar();