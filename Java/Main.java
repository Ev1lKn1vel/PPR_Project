import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.*;
import java.util.concurrent.*;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.stream.Collectors;

public class Main {

    public static char[] charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".toCharArray();
    private static final String charArray = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private static String HASHTYPE = "SHA-1";
    public static volatile boolean FOUND = false;
    private static MessageDigest digest;

    static {
        try {
            digest = MessageDigest.getInstance(HASHTYPE);
        } catch (NoSuchAlgorithmException e) {
            e.printStackTrace();
        }
    }


    public static void main(String[] args) throws NoSuchAlgorithmException, InterruptedException, ExecutionException {

        String password = "ZZZZ";

        byte[] hashedPassword = digest.digest(password.getBytes());
        int maxLength = 4;
//        benchMarkSerial(hashedPassword, maxLength);
//        benchMarkParallelExecutor(hashedPassword, maxLength);
        benchMarkParallelStream(hashedPassword, maxLength);

    }

    public static void benchMarkSerial(byte[] hashedPassword, int maxLength) {
        System.out.println("Starting Benchmark Serial\n");
        long start = System.currentTimeMillis();

        String guessedPW = guessPasswordSerial(hashedPassword, maxLength, "");

        long end = System.currentTimeMillis();
        System.out.println("total Time: " + (end - start) + "ms and password is " + guessedPW);
    }

    public static void benchMarkParallelExecutor(byte[] hashedPassword, int maxLength) throws ExecutionException, InterruptedException {
        //set up
        ExecutorService executorService = Executors.newFixedThreadPool(4);
        List<Callable<String>> callables = generateGuesserCallables(hashedPassword, maxLength);

        System.out.println("Starting Benchmark Parallel Executor\n");

        long start = System.currentTimeMillis();
        //Execution Exception means no result was returned from any of the callables
        String guessedPW = executorService.invokeAny(callables);

        long end = System.currentTimeMillis();
        System.out.println("total Time: " + (end - start) + "ms and password is " + guessedPW);
        executorService.shutdown();
    }

    public static void benchMarkParallelStream(byte[] hashedPassword, int maxLength) {
        System.out.println("Starting Benchmark Parallel Stream\n");

        long start = System.currentTimeMillis();

        String guessedPW = guessPasswordParellel(hashedPassword, maxLength, "");

        long end = System.currentTimeMillis();
        System.out.println("total Time: " + (end - start) + "ms and password is " + guessedPW);
    }

    // rekursive method generates StackOverflowError, therefore fixed for loops are used for benchmark
    public static String guessPasswordSerial(byte[] hashedPassword, int maxLength, String guessedPw) {
        //in most inner loop
        byte[] possibleSolution;
        String possiblePW;
        for (int first = 0; first < charList.length && maxLength > 0; first++) {
            possiblePW = "" + charList[first];
            possibleSolution = digest.digest(guessedPw.getBytes());
            if (Arrays.equals(hashedPassword, possibleSolution)) {
                return guessedPw;
            }
            for (int second = 0; second < charList.length && maxLength > 1; second++) {
                possiblePW = "" + charList[first] + charList[second];
                possibleSolution = digest.digest(guessedPw.getBytes());
                if (Arrays.equals(hashedPassword, possibleSolution)) {
                    return guessedPw;
                }
                for (int third = 0; third < charList.length && maxLength > 2; third++) {
                    possiblePW = "" + charList[first] + charList[second] + charList[third];
                    possibleSolution = digest.digest(guessedPw.getBytes());
                    if (Arrays.equals(hashedPassword, possibleSolution)) {
                        return guessedPw;
                    }
                    for (int fourth = 0; fourth < charList.length && maxLength > 3; fourth++) {
                        possiblePW = "" + charList[first] + charList[second] + charList[third] + charList[fourth];
                        possibleSolution = digest.digest(possiblePW.getBytes());
                        if (Arrays.equals(hashedPassword, possibleSolution)) {
                            return possiblePW;
                        }
                        for (int fifth = 0; fifth < charList.length && maxLength > 4; fifth++) {
                            possiblePW = "" + charList[first] + charList[second] + charList[third] + charList[fourth] + charList[fifth];
                            possibleSolution = digest.digest(possiblePW.getBytes());
                            if (Arrays.equals(hashedPassword, possibleSolution)) {
                                return possiblePW;
                            }
                        }
                    }
                }
            }
        }

        return "";
    }


    /* When speaking of the elements which are already being processed, it will wait for the completion of all of them,
     *  as the Stream API allows concurrent processing of data structures which are not intrinsically thread safe.
     *  It must ensure that all potential concurrent access has been finished before returning from the terminal operation.
     *
     * */
    public static String guessPasswordParellel(byte[] hashedPassword, int maxLength, String guessedPw) {

        return charArray.chars().parallel()
                .mapToObj(item -> recursiveLoop(maxLength - 1, (char) item + "", hashedPassword))
                .filter(item -> item != null && !item.isBlank())
                .findAny().orElse("NOTHING FOUND");
    }


    private static String recursiveLoop(int maxLength, String guessedPw, byte[] hashedPassword) {

        if (!FOUND) {

            try {
                MessageDigest digestInside = MessageDigest.getInstance(HASHTYPE);
                byte[] possibleSolution = digestInside.digest(guessedPw.getBytes());
                if (Arrays.equals(hashedPassword, possibleSolution)) {
                    FOUND = true;
                    return guessedPw;
                }

            } catch (Exception e) {
                return null;
            }
            if (maxLength > 0) {

                for (int i = 0; i < charList.length && !FOUND; i++) {
                    String tmp = recursiveLoop(maxLength - 1, guessedPw + charList[i], hashedPassword);
                    if (tmp != null && !tmp.isBlank()) {
                        return tmp;
                    }
                }
            }
        }
        return null;
    }

    public static List<Callable<String>> generateGuesserCallables(byte[] hashedPassword, int maxLength) throws ExecutionException, InterruptedException {

        List<Callable<String>> callables = new ArrayList<>();
        int loopLength = charArray.length();
        for (int i = 0; i < loopLength; i = i + 13) {
            TaskSplitter tmp = new TaskSplitter(charArray.substring(i, i + 13), maxLength, hashedPassword);
            callables.add(tmp);
        }

        return callables;
    }


}
