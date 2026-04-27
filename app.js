const rawData = window.LAWYER_UP_DATA;

const stats = ["credibility", "empathy", "risk", "judgePressure"];
const statLabels = {
  credibility: "Credibility",
  empathy: "Empathy",
  risk: "Risk",
  judgePressure: "Pressure",
};

const strategies = {
  mercy: {
    name: "Mercy",
    tag: "Humanize the client",
    copy: "Lean on context, remorse, and the cost of a harsh sentence.",
    modifiers: { credibility: -1, empathy: 8, risk: -2, judgePressure: 0 },
  },
  evidence: {
    name: "Evidence",
    tag: "Control the record",
    copy: "Make every fact feel orderly, documented, and hard to dismiss.",
    modifiers: { credibility: 8, empathy: -1, risk: -3, judgePressure: 2 },
  },
  technicality: {
    name: "Technicality",
    tag: "Attack the process",
    copy: "Pressure procedure, timing, and burden of proof at every turn.",
    modifiers: { credibility: 3, empathy: -4, risk: 1, judgePressure: -8 },
  },
};

const ranks = [
  { name: "Intern", min: 0 },
  { name: "Associate", min: 100 },
  { name: "Trial Lawyer", min: 260 },
  { name: "Senior Counsel", min: 520 },
  { name: "Defense Legend", min: 900 },
];

const badgeCatalog = {
  first_defense: { name: "First Defense", copy: "Completed your first case." },
  no_hints: { name: "No Hints", copy: "Won a trial without consulting." },
  perfect_mitigation: { name: "Perfect Mitigation", copy: "Scored 90 or better." },
  three_case_streak: { name: "Three Case Streak", copy: "Built a streak of 3 strong results." },
  balanced_advocate: { name: "Balanced Advocate", copy: "Ended with credibility and empathy both above 64." },
};

const state = {
  playerName: "",
  sessionScore: 0,
  career: defaultCareer(),
  crime: null,
  client: null,
  strategyKey: null,
  questions: [],
  currentIndex: 0,
  optionValues: [],
  decisions: [],
  trialStats: {},
  sentenceScore: 0,
  hintsLeft: 2,
  usedHints: 0,
  lastTrial: null,
};

const screens = {
  name: document.querySelector("#name-screen"),
  crime: document.querySelector("#crime-screen"),
  briefing: document.querySelector("#briefing-screen"),
  trial: document.querySelector("#trial-screen"),
  result: document.querySelector("#result-screen"),
  summary: document.querySelector("#summary-screen"),
};

function defaultCareer() {
  return {
    xp: 0,
    bestScore: 0,
    totalCases: 0,
    streak: 0,
    perfects: 0,
    badges: [],
    history: [],
  };
}

function storageKey(name) {
  return `lawyerup:${name.toLowerCase()}:career`;
}

function escapeHtml(value) {
  return String(value ?? "")
    .replaceAll("&", "&amp;")
    .replaceAll("<", "&lt;")
    .replaceAll(">", "&gt;")
    .replaceAll('"', "&quot;")
    .replaceAll("'", "&#039;");
}

function clamp(value, min = 0, max = 100) {
  return Math.max(min, Math.min(max, value));
}

function randomItem(items) {
  return items[Math.floor(Math.random() * items.length)];
}

function completeCrimes() {
  return rawData.crimes.filter((crime) => rawData.questions.filter((question) => question.crimeId === crime.id).length >= 10);
}

function showScreen(name) {
  Object.values(screens).forEach((screen) => {
    screen.classList.remove("active", "screen-enter");
  });
  screens[name].classList.add("active", "screen-enter");
  window.scrollTo({ top: 0, behavior: "smooth" });
}

function loadCareer(name) {
  try {
    const stored = JSON.parse(localStorage.getItem(storageKey(name)) || "null");
    return { ...defaultCareer(), ...stored, badges: stored?.badges || [], history: stored?.history || [] };
  } catch {
    return defaultCareer();
  }
}

function saveCareer() {
  if (!state.playerName) return;
  localStorage.setItem(storageKey(state.playerName), JSON.stringify(state.career));
}

function currentRank() {
  return ranks.reduce((active, rank) => (state.career.xp >= rank.min ? rank : active), ranks[0]);
}

function nextRank() {
  return ranks.find((rank) => rank.min > state.career.xp) || ranks[ranks.length - 1];
}

function updateChrome() {
  const rank = currentRank();
  const next = nextRank();
  const previous = ranks[Math.max(0, ranks.indexOf(rank))];
  const span = Math.max(1, next.min - previous.min);
  const progress = next === rank ? 100 : clamp(((state.career.xp - previous.min) / span) * 100);

  document.querySelector("#career-rank").textContent = rank.name;
  document.querySelector("#session-score").textContent = state.sessionScore;
  document.querySelector("#best-score").textContent = state.career.bestScore;
  document.querySelector("#career-streak").textContent = state.career.streak;
  document.querySelector("#rank-label").textContent = rank.name;
  document.querySelector("#rank-progress-label").textContent = next === rank
    ? `${state.career.xp} XP`
    : `${state.career.xp} / ${next.min} XP`;
  document.querySelector("#rank-progress").style.width = `${progress}%`;
  document.querySelector("#career-cases").textContent = state.career.totalCases;
  document.querySelector("#badge-count").textContent = state.career.badges.length;
  document.querySelector("#perfect-count").textContent = state.career.perfects;
  renderBadges();
}

function renderBadges() {
  const rack = document.querySelector("#badge-rack");
  const earned = state.career.badges.map((id) => badgeCatalog[id]).filter(Boolean);
  rack.innerHTML = earned.length
    ? earned.map((badge) => `<span title="${escapeHtml(badge.copy)}">${escapeHtml(badge.name)}</span>`).join("")
    : "<em>No badges yet. The courtroom is waiting.</em>";
}

function renderCrimeDesk() {
  const grid = document.querySelector("#crime-grid");
  const crimes = completeCrimes();
  document.querySelector("#case-count-label").textContent = `${crimes.length} complete categories`;
  grid.innerHTML = "";

  crimes.forEach((crime) => {
    const clients = rawData.clients.filter((client) => client.crimeId === crime.id);
    const difficulty = difficultyFor(crime.hostVal);
    const button = document.createElement("button");
    button.type = "button";
    button.className = "crime-card";
    button.innerHTML = `
      <span class="file-tab">Case ${crime.id}</span>
      <strong>${escapeHtml(crime.name)}</strong>
      <span>${clients.length} possible clients</span>
      <span>${difficulty} difficulty</span>
    `;
    button.addEventListener("click", () => openBriefing(crime.id));
    grid.append(button);
  });
}

function difficultyFor(hostVal) {
  if (hostVal >= 8) return "Severe";
  if (hostVal >= 5) return "High";
  return "Measured";
}

function openBriefing(crimeId) {
  state.crime = rawData.crimes.find((crime) => crime.id === crimeId);
  state.client = randomItem(rawData.clients.filter((client) => client.crimeId === crimeId));
  state.questions = rawData.questions
    .filter((question) => question.crimeId === crimeId)
    .sort((a, b) => a.questionNo - b.questionNo);
  state.strategyKey = null;
  renderBriefing();
  showScreen("briefing");
}

function renderBriefing() {
  const client = state.client;
  const details = [
    ["Client", `${client.firstName} ${client.secondName}`],
    ["Charge", state.crime.name],
    ["Age", client.age],
    ["Country", client.country],
    ["Occupation", client.occupation],
    ["Criminal history", client.criminalHistory],
    ["Case difficulty", difficultyFor(state.crime.hostVal)],
  ];

  document.querySelector("#briefing-details").innerHTML = details
    .map(([label, value]) => `<div><dt>${escapeHtml(label)}</dt><dd>${escapeHtml(value)}</dd></div>`)
    .join("");

  document.querySelector("#evidence-strip").innerHTML = [
    "Client interview sealed",
    `${state.questions.length} decision points`,
    `Hostility factor ${state.crime.hostVal}`,
    `Profile factor ${client.hostVal}`,
  ].map((item) => `<span>${escapeHtml(item)}</span>`).join("");

  const grid = document.querySelector("#strategy-grid");
  grid.innerHTML = Object.entries(strategies).map(([key, strategy]) => `
    <button class="strategy-card" type="button" data-strategy="${key}">
      <span>${escapeHtml(strategy.tag)}</span>
      <strong>${escapeHtml(strategy.name)}</strong>
      <em>${escapeHtml(strategy.copy)}</em>
    </button>
  `).join("");

  grid.querySelectorAll("button").forEach((button) => {
    button.addEventListener("click", () => startTrial(button.dataset.strategy));
  });
}

function startTrial(strategyKey) {
  state.strategyKey = strategyKey;
  state.currentIndex = 0;
  state.optionValues = [];
  state.decisions = [];
  state.sentenceScore = 0;
  state.hintsLeft = 2;
  state.usedHints = 0;
  state.trialStats = {
    credibility: 50,
    empathy: 50,
    risk: 50,
    judgePressure: 50,
  };

  Object.entries(strategies[strategyKey].modifiers).forEach(([key, value]) => {
    state.trialStats[key] = clamp(state.trialStats[key] + value);
  });

  renderTrialShell();
  renderQuestion();
  showScreen("trial");
}

function renderTrialShell() {
  document.querySelector("#trial-client-name").textContent = `${state.client.firstName} ${state.client.secondName}`;
  document.querySelector("#trial-strategy-label").textContent = `${strategies[state.strategyKey].name} strategy active`;
  renderStats();
}

function renderStats(deltas = {}) {
  const stack = document.querySelector("#stat-stack");
  stack.innerHTML = stats.map((key) => {
    const value = state.trialStats[key];
    const delta = deltas[key] || 0;
    const deltaClass = delta > 0 ? "positive" : delta < 0 ? "negative" : "";
    const deltaText = delta ? `${delta > 0 ? "+" : ""}${delta}` : "";
    return `
      <div class="stat-meter ${deltaClass}">
        <div>
          <span>${statLabels[key]}</span>
          <strong>${value}</strong>
          <em>${deltaText}</em>
        </div>
        <div class="meter-track"><span style="width: ${value}%"></span></div>
      </div>
    `;
  }).join("");
}

function currentQuestion() {
  return state.questions[state.currentIndex];
}

function questionValues(question) {
  return [question.hostVal1, question.hostVal2, question.hostVal3];
}

function optionText(question, index) {
  return [question.option1, question.option2, question.option3][index];
}

function optionValue(question, index) {
  return questionValues(question)[index];
}

function renderQuestion() {
  const question = currentQuestion();
  document.querySelector("#progress-label").textContent = `Decision ${state.currentIndex + 1} of ${state.questions.length}`;
  document.querySelector("#question-text").textContent = question.question;
  document.querySelector("#hints-left").textContent = state.hintsLeft;
  document.querySelector("#sentence-profile").textContent = state.sentenceScore;
  document.querySelector("#hint-text").classList.add("hidden");
  document.querySelector("#hint-text").textContent = "";
  document.querySelector("#reaction-panel").classList.add("hidden");
  document.querySelector("#hint-button").disabled = state.hintsLeft <= 0;

  const options = document.querySelector("#options");
  options.innerHTML = "";
  [0, 1, 2].forEach((index) => {
    const value = optionValue(question, index);
    const preview = previewFor(value, questionValues(question));
    const button = document.createElement("button");
    button.type = "button";
    button.className = "option-button";
    button.innerHTML = `
      <span class="option-index">Option ${index + 1}</span>
      <strong>${escapeHtml(optionText(question, index))}</strong>
      <em>${escapeHtml(preview)}</em>
    `;
    button.addEventListener("click", () => chooseOption(index));
    options.append(button);
  });
}

function previewFor(value, values) {
  const min = Math.min(...values);
  const max = Math.max(...values);
  if (value === min) return "Likely softens the sentence, but may require finesse.";
  if (value === max) return "High-risk stance with a harsher sentencing profile.";
  return "Balanced move with mixed courtroom reaction.";
}

function deltasFor(value, values) {
  const min = Math.min(...values);
  const max = Math.max(...values);
  const range = Math.max(1, max - min);
  const severity = (value - min) / range;
  const strategy = state.strategyKey;
  const deltas = {
    credibility: Math.round(5 - severity * 8),
    empathy: Math.round(6 - severity * 10),
    risk: Math.round(severity * 12 - 5),
    judgePressure: Math.round(severity * 10 - 4),
  };

  if (strategy === "mercy") {
    deltas.empathy += 3;
    deltas.credibility -= severity > 0.65 ? 2 : 0;
  }
  if (strategy === "evidence") {
    deltas.credibility += 3;
    deltas.risk -= severity < 0.35 ? 2 : 0;
  }
  if (strategy === "technicality") {
    deltas.judgePressure -= 3;
    deltas.risk += severity > 0.65 ? 2 : 0;
  }

  return deltas;
}

function chooseOption(index) {
  const question = currentQuestion();
  const value = optionValue(question, index);
  const values = questionValues(question);
  const deltas = deltasFor(value, values);

  Object.entries(deltas).forEach(([key, delta]) => {
    state.trialStats[key] = clamp(state.trialStats[key] + delta);
  });

  const quality = Math.max(...values) - value;
  state.optionValues.push(value);
  state.sentenceScore += value;
  state.decisions.push({
    question: question.question,
    option: optionText(question, index),
    value,
    quality,
    deltas,
  });

  renderReaction(value, values, deltas);
  renderStats(deltas);

  window.setTimeout(() => {
    state.currentIndex += 1;
    if (state.currentIndex >= state.questions.length) {
      finishTrial();
    } else {
      renderQuestion();
    }
  }, 1050);
}

function renderReaction(value, values, deltas) {
  const min = Math.min(...values);
  const max = Math.max(...values);
  const title = value === min ? "The bench leans in." : value === max ? "The room tightens." : "The record shifts.";
  const copy = value === min
    ? "Your argument gives the judge a cleaner path to mitigation."
    : value === max
      ? "The prosecution finds pressure in that answer. You may pay for it at sentencing."
      : "A useful move, but not decisive enough to fully control the narrative.";

  document.querySelector("#reaction-title").textContent = title;
  document.querySelector("#reaction-copy").textContent = copy;
  document.querySelector("#delta-row").innerHTML = stats.map((key) => {
    const delta = deltas[key] || 0;
    return `<span class="${delta >= 0 ? "positive" : "negative"}">${statLabels[key]} ${delta > 0 ? "+" : ""}${delta}</span>`;
  }).join("");
  document.querySelector("#reaction-panel").classList.remove("hidden");
}

function useHint() {
  if (state.hintsLeft <= 0) return;

  const question = currentQuestion();
  const values = questionValues(question);
  const bestIndex = values.indexOf(Math.min(...values));
  state.hintsLeft -= 1;
  state.usedHints += 1;

  const hint = document.querySelector("#hint-text");
  hint.textContent = `Consultant read: Option ${bestIndex + 1} gives the cleanest mitigation angle because it lowers the sentencing profile for this decision.`;
  hint.classList.remove("hidden");
  document.querySelector("#hints-left").textContent = state.hintsLeft;
  document.querySelector("#sentence-profile").textContent = state.sentenceScore;
  document.querySelector("#hint-button").disabled = state.hintsLeft <= 0;
}

function sentenceYears(optionSum, crimeVal, clientVal) {
  const months = Math.pow(optionSum, 0.8) * crimeVal * Math.pow(clientVal, 1 / 5);
  return Math.max(1, Math.round(months / 12));
}

function trialScore(maxYears, minYears, actualYears) {
  if (maxYears === minYears) return 100;
  const statBonus = Math.round((state.trialStats.credibility + state.trialStats.empathy - state.trialStats.risk - state.trialStats.judgePressure) / 10);
  const raw = ((maxYears - actualYears) / (maxYears - minYears)) * 100 + statBonus;
  return clamp(Math.round(raw));
}

function finishTrial() {
  const optionSum = state.optionValues.reduce((sum, value) => sum + value, 0);
  const valueSets = state.questions.map(questionValues);
  const maxSum = valueSets.reduce((sum, values) => sum + Math.max(...values), 0);
  const minSum = valueSets.reduce((sum, values) => sum + Math.min(...values), 0);
  const finalYears = sentenceYears(optionSum, state.crime.hostVal, state.client.hostVal);
  const maxYears = sentenceYears(maxSum, state.crime.hostVal, state.client.hostVal);
  const minYears = sentenceYears(minSum, state.crime.hostVal, state.client.hostVal);
  const reducedYears = Math.max(0, maxYears - finalYears);
  const score = trialScore(maxYears, minYears, finalYears);
  const strongest = [...state.decisions].sort((a, b) => b.quality - a.quality)[0];
  const weakest = [...state.decisions].sort((a, b) => a.quality - b.quality)[0];
  const newBadges = awardCareer(score);

  state.lastTrial = {
    score,
    finalYears,
    reducedYears,
    maxYears,
    crime: state.crime.name,
    client: `${state.client.firstName} ${state.client.secondName}`,
    strategy: strategies[state.strategyKey].name,
    strongest,
    weakest,
    badges: newBadges,
  };

  state.sessionScore += score;
  saveCareer();
  updateChrome();
  renderVerdict();
  showScreen("result");
}

function awardCareer(score) {
  const earned = [];
  state.career.totalCases += 1;
  state.career.xp += score;
  state.career.bestScore = Math.max(state.career.bestScore, score);
  state.career.streak = score >= 70 ? state.career.streak + 1 : 0;
  state.career.perfects += score >= 90 ? 1 : 0;
  state.career.history.unshift({
    score,
    crime: state.crime.name,
    client: `${state.client.firstName} ${state.client.secondName}`,
    strategy: strategies[state.strategyKey].name,
    date: new Date().toLocaleDateString(),
  });
  state.career.history = state.career.history.slice(0, 8);

  const checks = {
    first_defense: state.career.totalCases >= 1,
    no_hints: state.usedHints === 0,
    perfect_mitigation: score >= 90,
    three_case_streak: state.career.streak >= 3,
    balanced_advocate: state.trialStats.credibility >= 65 && state.trialStats.empathy >= 65,
  };

  Object.entries(checks).forEach(([id, passed]) => {
    if (passed && !state.career.badges.includes(id)) {
      state.career.badges.push(id);
      earned.push(id);
    }
  });

  return earned;
}

function renderVerdict() {
  const trial = state.lastTrial;
  document.querySelector("#result-title").textContent = `${trial.client.split(" ")[0]}'s sentence: ${trial.finalYears} years`;
  document.querySelector("#result-copy").textContent = `The maximum exposure was ${trial.maxYears} years. Your ${trial.strategy} strategy reduced the sentence by ${trial.reducedYears} years.`;
  document.querySelector("#trial-score").textContent = "0";
  document.querySelector("#grade-label").textContent = gradeFor(trial.score);
  document.querySelector("#final-years").textContent = `${trial.finalYears} years`;
  document.querySelector("#reduced-years").textContent = `${trial.reducedYears} years`;
  document.querySelector("#strategy-result").textContent = trial.strategy;
  document.querySelector("#strongest-move").textContent = trial.strongest ? trial.strongest.option : "None";
  document.querySelector("#weakest-move").textContent = trial.weakest ? trial.weakest.option : "None";
  document.querySelector("#unlock-row").innerHTML = trial.badges.length
    ? trial.badges.map((id) => `<span>Unlocked: ${escapeHtml(badgeCatalog[id].name)}</span>`).join("")
    : "<span>Career progress archived</span>";
  animateCount(document.querySelector("#trial-score"), trial.score, "/100");
}

function gradeFor(score) {
  if (score >= 90) return "Legendary defense";
  if (score >= 75) return "Commanding defense";
  if (score >= 55) return "Credible defense";
  return "Rough hearing";
}

function animateCount(element, target, suffix = "") {
  const start = performance.now();
  const duration = 900;
  function tick(now) {
    const progress = clamp((now - start) / duration, 0, 1);
    element.textContent = `${Math.round(target * progress)}${suffix}`;
    if (progress < 1) requestAnimationFrame(tick);
  }
  requestAnimationFrame(tick);
}

function renderSummary() {
  const rank = currentRank();
  document.querySelector("#summary-title").textContent = `${state.playerName}, ${rank.name}`;
  document.querySelector("#summary-copy").textContent = `Session score ${state.sessionScore}. Career XP ${state.career.xp}. Best trial ${state.career.bestScore}.`;
  document.querySelector("#trial-list").innerHTML = state.career.history.length
    ? state.career.history.map((trial) => `
      <div>
        <span>${escapeHtml(trial.date)} / ${escapeHtml(trial.crime)}</span>
        <strong>${trial.score}/100</strong>
        <span>${escapeHtml(trial.client)} / ${escapeHtml(trial.strategy)}</span>
      </div>
    `).join("")
    : "<p>No cases archived yet.</p>";
}

document.querySelector("#name-form").addEventListener("submit", (event) => {
  event.preventDefault();
  const name = document.querySelector("#player-name").value.trim();
  if (!name) return;

  state.playerName = name;
  state.sessionScore = 0;
  state.career = loadCareer(name);
  document.querySelector("#welcome-label").textContent = `Welcome back, ${name}`;
  updateChrome();
  renderCrimeDesk();
  showScreen("crime");
});

document.querySelector("#hint-button").addEventListener("click", useHint);
document.querySelector("#next-trial").addEventListener("click", () => {
  renderCrimeDesk();
  showScreen("crime");
});
document.querySelector("#end-session").addEventListener("click", () => {
  renderSummary();
  showScreen("summary");
});
document.querySelector("#restart").addEventListener("click", () => {
  document.querySelector("#player-name").value = "";
  state.playerName = "";
  state.sessionScore = 0;
  state.career = defaultCareer();
  updateChrome();
  showScreen("name");
});
document.querySelector("#back-to-desk").addEventListener("click", () => {
  renderCrimeDesk();
  showScreen("crime");
});

updateChrome();
