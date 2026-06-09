document.addEventListener('DOMContentLoaded', () => {
    
    const navbar = document.getElementById('navbar');
    if (navbar) {
        window.addEventListener('scroll', () => {
            if (window.scrollY > 50) {
                navbar.classList.add('scrolled');
            } else {
                navbar.classList.remove('scrolled');
            }
        });
    }

    const loginBtn = document.getElementById('login-btn');
    const phoneInput = document.getElementById('phone-input');
    const loginSection = document.getElementById('login-section');
    const cabinetData = document.getElementById('cabinet-data');
    const loginError = document.getElementById('login-error');
    const logoutBtn = document.getElementById('logout-btn');

    let currentBaseDate = new Date(); 
    let clientTrainings = []; 

    if (loginBtn) {
        loginBtn.addEventListener('click', async () => {
            const phone = phoneInput.value.trim();
            loginError.textContent = '';

            if (!phone) {
                loginError.textContent = "Будь ласка, введіть номер телефону";
                return;
            }

            try {
                const response = await fetch('schedule.json'); 
                if (!response.ok) throw new Error("Файл бази даних не знайдено");
                
                const allData = await response.json();

                clientTrainings = allData.filter(c => c.ClientPhone === phone);

                if (clientTrainings.length > 0) {
                    showCabinet(clientTrainings);
                } else {
                    loginError.textContent = "Тренувань для цього номеру не знайдено";
                }
                
            } catch (error) {
                console.error("Помилка:", error);
                loginError.textContent = "Помилка зв'язку з базою даних";
            }
        });
    }

    function showCabinet(trainings) {
        loginSection.style.display = 'none';
        cabinetData.style.display = 'block';

        document.getElementById('cab-name').textContent = trainings[0].ClientName;
        
        if(trainings[0].TrainingDate) {
            currentBaseDate = new Date(trainings[0].TrainingDate.split(' ')[0]);
        }
        
        renderWeek(currentBaseDate);
    }

    function getMonday(d) {
        const date = new Date(d);
        const day = date.getDay();
        const diff = date.getDate() - day + (day === 0 ? -6 : 1);
        return new Date(date.setDate(diff));
    }

    function renderWeek(baseDate) {
        const monday = getMonday(baseDate);
        const datesContainer = document.getElementById('week-dates');
        const slotsContainer = document.getElementById('week-slots');
        const monthYearDisplay = document.getElementById('month-year-display');
        const detailsContainer = document.getElementById('training-details');
        
        datesContainer.innerHTML = '';
        slotsContainer.innerHTML = '';
        detailsContainer.style.display = 'none';

        const options = { month: 'long', year: 'numeric' };
        monthYearDisplay.textContent = monday.toLocaleDateString('uk-UA', options).replace(' р.', '');

        for (let i = 0; i < 7; i++) {
            let iterationDate = new Date(monday);
            iterationDate.setDate(monday.getDate() + i);
            
            const dateStr = iterationDate.getFullYear() + '-' + 
                            String(iterationDate.getMonth() + 1).padStart(2, '0') + '-' + 
                            String(iterationDate.getDate()).padStart(2, '0');
            
            const dayTrainings = clientTrainings.filter(t => t.TrainingDate.startsWith(dateStr));
            
            const dateDiv = document.createElement('div');
            dateDiv.className = 'date-circle';
            dateDiv.textContent = iterationDate.getDate();
            
            const slotDiv = document.createElement('div');
            slotDiv.className = 'slot-card';
            
            if (dayTrainings.length > 0) {
                dateDiv.classList.add('has-training');
                slotDiv.classList.add('has-data');
                
                const tInfo = dayTrainings[0];
                const timeStr = tInfo.TrainingDate.split(' ')[1] || "Час не вказано";
                
                slotDiv.innerHTML = `
                    <div class="slot-title">🎯 ${tInfo.Activity}</div>
                    <div class="slot-time">🕒 ${timeStr}</div>
                `;

                const showDetails = () => {
                    document.querySelectorAll('.date-circle').forEach(el => el.classList.remove('selected'));
                    document.querySelectorAll('.slot-card').forEach(el => el.classList.remove('selected'));
                    
                    dateDiv.classList.add('selected');
                    slotDiv.classList.add('selected');
                    
                    detailsContainer.style.display = 'block';
                    detailsContainer.innerHTML = `
                        <h3 style="margin-bottom: 15px; color: var(--text-main);">Деталі тренування</h3>
                        <p><strong>Напрямок:</strong> ${tInfo.Activity}</p>
                        <p><strong>Час проведення:</strong> ${timeStr} (${iterationDate.toLocaleDateString('uk-UA')})</p>
                        <p><strong>Тренер:</strong> ${tInfo.Trainer.FullName} (${tInfo.Trainer.Specialization})</p>
                        <p><strong>Зв'язок з тренером:</strong> <a href="tel:${tInfo.Trainer.ContactPhone}">${tInfo.Trainer.ContactPhone}</a></p>
                    `;
                };
                
                dateDiv.addEventListener('click', showDetails);
                slotDiv.addEventListener('click', showDetails);
            }
            
            datesContainer.appendChild(dateDiv);
            slotsContainer.appendChild(slotDiv);
        }
    }

    document.getElementById('prev-week')?.addEventListener('click', () => {
        currentBaseDate.setDate(currentBaseDate.getDate() - 7);
        renderWeek(currentBaseDate);
    });
    
    document.getElementById('next-week')?.addEventListener('click', () => {
        currentBaseDate.setDate(currentBaseDate.getDate() + 7);
        renderWeek(currentBaseDate);
    });

    if (logoutBtn) {
        logoutBtn.addEventListener('click', () => {
            cabinetData.style.display = 'none';
            loginSection.style.display = 'block';
            phoneInput.value = '';
            clientTrainings = [];
        });
    }
});