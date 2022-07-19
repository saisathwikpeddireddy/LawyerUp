# LawyerUp Game PRD

[https://github.com/saisathwikpeddireddy/LawyerUp](https://github.com/saisathwikpeddireddy/LawyerUp)

# Background

<aside>
🌻 Why this need arises?

</aside>

This is an idea that popped up in my head after seeing [this](https://www.youtube.com/watch?v=OMr6zCXwuns) youtube video by Vsauce2 titled The Punishment Algorithm.

[https://www.youtube.com/watch?v=OMr6zCXwuns](https://www.youtube.com/watch?v=OMr6zCXwuns)

What interested me is how complex and complicated the idea of calculating the sentence period is … Can we even call it a fair judicial system if the length of sentence for a crime with the same facts has a significant variation when evaluated by different judges or courtrooms, etc.

This variation has given rise to more algorithmic thinking of calculating the length of sentences and hence allowing for a higher degree of objectivity.

<aside>
📌 ***Through this project, this algorithmic thinking to determine punishments is exactly what I would like to explore in depth while also delivering a high level of interactivity and fun experience for the users of this project.***

</aside>

---

# What?

<aside>
💡 What am I building to explore the above-highlighted concept?

</aside>

This project will start out as a simple console application and might take shape of a web/mobile application based on the response and other external factors.

This will be presented in the format of a text-based RPG game where the user will be playing as a lawyer with his main goal being to reduce the sentence length by making appropriate choices and defending a guilty client.

The length of the sentences will be calculated by ”The Punishment algorithm“which takes the decisions made by the user into account.

The user is also given a score after every trial which will be calculated by the potential months of sentence reduced for the client.

---

# Product Requirements

<aside>
⚙ What will the product include at a more granular level for the user to experience during his journey with the product?

</aside>

- Have at least 10 decision points for the user to make during each trial
- Have at least 3 options the game can turn at each decision point
- Have at least 3 different types of crimes for the user to choose clients from in the initial decision point
- Give 2 hints for the user to use at 2 different decision points of his choosing that will suggest the option that might result in the least sentence down the line
- Display the score to the user after the end of each trial along with the length of the sentence given
    - Trial_Score(in yrs)=

$$
{{(Max Sentence-Actual Sentence)}\above 2 pt{(Max Sentence - Min Sentence)}}*100
$$

- Sentence_Length(in yrs)=

$$
\Sigma{optionVal}^{0.8}\*crimeVal^{1.5}\*\sqrt[3]{clientVal}\above 2pt 12 
$$

- Allow the user to have multiple client trials in a single session
- The score should be continuous for each session, i.e it should not restart from zero for each trial but add to the score of the previous trial
- Each client chosen should have randomisation for the following attributes:
    1. First Name
    2. Last Name
    3. Country
    4. Age
    5. Gender
    6. Marital Status
    7. Crime
    8. Occupation
    9. Criminal History
    10. Hostility Value(1-10) — This determines how much might the effect of client demographic be on the length of the sentence
    11. Appearance (Console characters or Text Description)
    
- Ask the user to pick a username to which they will be referred for the entire session. If the username already exists give a prompt asking them if they want to create a new one or Continue as that was them before. (See if a password system can be done at a later point in time)
- Maintain the score data of the users for all the previous sessions to display messages like Highest Trial Score, Highest Session Score, Avg Trial Score, etc.

---
