## esay
- [the unexpected outcoms of code review](http://michaelrbernste.in/2013/10/16/the-unexpected-outcomes-of-code-review.html)
- [Beware of Developers Who Do Negative Work](http://blog.professorbeekums.com/2016/12/beware-of-developers-who-do-negative.html)
- [Mastering Programming](https://www.prod.facebook.com/notes/kent-beck/mastering-programming/1184427814923414?utm_source=Dev+Mastery+Newsletter&utm_campaign=724eefb4d1-EMAIL_CAMPAIGN_2016_12_21&utm_medium=email&utm_term=0_49661ec53b-724eefb4d1-163381209)

## Hackable
+ Code Health
  - Readable Code
    - Code Style: [google code style guid](https://github.com/google/styleguide)
    - [Code Review](https://en.wikipedia.org/wiki/Code_review)
  - Presubmit Test
  - Single Branch And Reducing Risk
  - Loose Couping
    - SOLID
  - Aggressively Reduce Technical Debt
    - dependency enforcement
+ Debuggability
  - Principle
    - Debuggability means being able to easily find what’s wrong with a piece of software
      -  through logs, statistics or debugger tools
    - Debuggability doesn’t happen by accident: you need to design it into your product
  - Running on Localhost
    - need some kind of script that brings up your server stack on localhost
  - Debugging Mobile Apps
    - mobile is hard
    - unit tests are great for hackability here
    - [Test-Drive Your Android Code:Robolectric](http://robolectric.org/)
    - [Android Testing Support Library:Espresso test](https://google.github.io/android-testing-support-library/docs/espresso/basics/index.html)
    - [Create and run unit tests, performance tests, and UI tests for your Xcode project: XCTest](https://developer.apple.com/reference/xctest)
    - [iOS UI Automation Test Framework:EarlGrey](https://github.com/google/EarlGrey)
  - When Debugging gets Tricky
    - debugging flags and instructions
    - you need to build these kinds of things into your product
  - Proper Logging
    - It’s hackability to get the right logs when you need them
    - Logs are also useful for development
    - [Optimal Logging](https://testing.googleblog.com/2013/06/optimal-logging.html)
  - Monitoring and Statistics
    - [monitoring tools such as Stackdriver for Google Cloud are excellent](https://cloud.google.com/stackdriver/)
  - System Under Test (SUT) Size
    - [Just Say No to More End-to-End Tests](https://testing.googleblog.com/2015/04/just-say-no-to-more-end-to-end-tests.html)
+ Infrastructure
  - Build Systems Speed
    - Replace make with ninja
    - Use the gold linker instead of ld
    - Detect and delete dead code in your project
    - Reduce the number of dependencies
    - enforce dependency rules so new ones are not added lightly
    - Give the developers faster machines
    - Use distributed build, which is available with many open-source continuous integration systems
  - feedback cycles kill hackability, for many reasons:
    - Build and test times longer than a handful of seconds cause many developers’ minds to wander, taking them out of the zone.
    - Excessive build or release times* makes tinkering and refactoring much harder.
  - The main axes of improvement are:
    - Reduce the amount of code being compiled.
    - Replace tools with faster counterparts.
    - Increase processing power, maybe through parallelization or distributed systems.
  - Continuous Integration and Presubmit Queues
    - You should build and run tests on all platforms you release on. 
    - At a minimum, you should build and test on all platforms, but it’s even better if you do it in presubmit.
    - Chromium:It has developed over the years so that a normal patch builds and tests on about 30 different build configurations before commit. 
  - Test Speed:
    -  if it takes more than a minute to execute, its value is greatly diminished
    - Sharding and parallelization.
    - Continuously measure how long it takes to run one build+test cycle in your continuous build, and have someone take action when it gets slower.
    - Remove tests that don’t pull their weight. 
    - If you have tests that bring up a local server stack, for instance inter-server integration tests, making your servers boot faster is going to make the tests faster as well. 
  - Workflow Speed
    - You need to keep track of your core workflows as your project grows. 
    - Your version control system may work fine for years, but become too slow once the project becomes big enough.
  - Release Often
    - It aids hackability to [deploy to real users as fast as possible](https://en.wikipedia.org/wiki/Release_early,_release_often).
  - Easy Reverts:
    - If you look in the commit log for the Chromium project, you will see that a significant percentage of the commits are reverts of a previous commits.
  - Performance Tests: Measure Everything:
    - Is it critical that your app starts up within a second? 
    - Should your app always render at 60 fps when it’s scrolled up or down?
    - Should your web server always serve a response within 100 ms?
    - Should your mobile app be smaller than 8 MB? 

#### [Just Say No to More End-to-End Tests](https://testing.googleblog.com/2015/04/just-say-no-to-more-end-to-end-tests.html)

Good ideas often fail in practice, and in the world of testing, one pervasive good idea that often fails in practice is a testing strategy built around end-to-end tests.

What Went Wrong for End-to-End test:
- The team completed their coding milestone a week late (and worked a lot of overtime). 
- Finding the root cause for a failing end-to-end test is painful and can take a long time. 
- Partner failures and lab failures ruined the test results on multiple days. 
- Many smaller bugs were hidden behind bigger bugs. 
- End-to-end tests were flaky at times. 
- Developers had to wait until the following day to know if a fix worked or not. 

The True Value of Tests:
- A failing test does not directly benefit the user. 
- A bug fix directly benefits the user.

Building the Right Feedback Loop:
- It's fast
- It's reliable(smaller)
- It isolates failures

Think Smaller, Not Larger

##### Unit Test
Unit tests take a small piece of the product and test that piece in isolation. 
- Unit tests are fast.
- Unit tests are reliable.
- Unit tests isolate failures.

Writing effective unit tests requires skills in areas:
- dependency management
- mocking
- hermetic testing

With end-to-end tests, you have to wait: first for the entire product to be built, then for it to be deployed, and finally for all end-to-end tests to run.

Although end-to-end tests do a better job of **simulating real user scenarios**, this advantage quickly becomes outweighed by all the disadvantages of the end-to-end feedback loop:
- NOT fast
- NOT Reliable
- NOT Isolates Failures


##### Integration Tests
Unit tests do have one major disadvantage: even if the units work well in isolation, you do not know if they work well together. 

But even then, you do not necessarily need end-to-end tests. For that, you can use an integration test.

An integration test takes a small group of units, often two units, and tests their behavior as a whole, verifying that they coherently work together.

##### Testing Pyramid
Even with both unit tests and integration tests, you probably still will want a small number of end-to-end tests to verify the system as a whole.

To find the right balance between all three test types, the best visual aid to use is the testing pyramid:
![](https://2.bp.blogspot.com/-YTzv_O4TnkA/VTgexlumP1I/AAAAAAAAAJ8/57-rnwyvP6g/s1600/image02.png)

As a good first guess, Google often suggests a 70/20/10 split: 70% unit tests, 20% integration tests, and 10% end-to-end tests.


#### [Optimal Logging](https://testing.googleblog.com/2013/06/optimal-logging.html)

##### Channeling Goldilocks:

Massive, disk-quota burning logs are a clear indicator that little thought was put in to logging.
- Never log too much:

Goals of logging:
- help with bug investigation
- event confirmation

If your log can’t explain the cause of a bug or whether a certain transaction took place, you are logging too little.
- The only thing worse than logging too much is logging too little.

Good things to log:
- Important startup configuration
- Errors
- Warnings
- Changes to persistent data
- Requests and responses between major system components
- Significant state changes
- User interactions
- Calls with a known risk of failure
- Waits on conditions that could take measurable time to satisfy
- Periodic progress during long-running tasks
- Significant branch points of logic and conditions that led to the branch
- Summaries of processing steps or events from high level functions - Avoid logging every step of a complex process in low-level functions.

Bad things to log:
- Function entry - Don’t log a function entry unless it is significant or logged at the debug level.
- Data within a loop - Avoid logging from many iterations of a loop. It is OK to log from iterations of small loops or to log periodically from large loops.
- Content of large messages or files - Truncate or summarize the data in some way that will be useful to debugging.
- Benign errors - Errors that are not really errors can confuse the log reader. This sometimes happens when exception handling is part of successful execution flow.
- Repetitive errors - Do not repetitively log the same or similar error. This can quickly fill a log and hide the actual cause. Frequency of error types is best handled by monitoring. Logs only need to capture detail for some of those errors.

##### There is More Than One Level
Test logs should always contain:
- Test execution environment
- Initial state
- Setup steps
- Test case steps
- Interactions with the system
- Expected results
- Actual results
- Teardown steps

##### Conditional Verbosity With Temporary Log Queues
to create temporary, in-memory log queues. Throughout processing of a transaction, append verbose details about each step to the queue. If the transaction completes successfully, discard the queue and log a summary. If an error is encountered, log the content of the entire queue and the error.

##### Failures and Flakiness Are Opportunities
If you have a hard time determining the cause of an error, it's a great opportunity to improve your logging. Before fixing the problem, fix your logging so that the logs clearly show the cause.

##### Might As Well Log Performance Data
Logged timing data can help debug performance issues. 

##### Following the Trail Through Many Threads and Processes
You should create unique identifiers for transactions that involve processing across many threads and/or processes

##### Monitoring and Logging Complement Each Other
a monitoring alert is simply a trigger for you to start an investigation. Monitoring shows the symptoms of problems. Logs provide details and state on individual transactions