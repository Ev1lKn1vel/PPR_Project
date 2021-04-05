import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;
import java.util.Arrays;
import java.util.concurrent.Callable;

public class TaskSplitter implements Callable<String> {

    public static char[] charList = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".toCharArray();
    private static String HASHTYPE = "SHA-1";
    private MessageDigest digest;

    private String chunk;
    private int maxLength;
    private byte[] hashedPassword;

    public TaskSplitter(String chunk, int maxLength, byte[] hashedPassword) {
        this.chunk = chunk;
        this.maxLength = maxLength;
        this.hashedPassword = hashedPassword;
    }

    private String recursiveLoop(int maxLength, String guessedPw, byte[] hashedPassword) {
            try {
                byte[] possibleSolution = digest.digest(guessedPw.getBytes());
                if (Arrays.equals(hashedPassword, possibleSolution)) {
                    return guessedPw;
                }

            } catch (Exception e) {
                return "";
            }
            if (maxLength > 0) {

                for (int i = 0; i < charList.length; i++) {
                    String tmp = recursiveLoop(maxLength - 1, guessedPw + charList[i], hashedPassword);
                    if (tmp != null && !tmp.isBlank()) {
                        return tmp;
                    }
                }
            }
        return "";
    }


    @Override
    public String call() throws Exception {

        digest = MessageDigest.getInstance(HASHTYPE);

        for(int i = 0; i < chunk.length() ; i++){
            String tmp = recursiveLoop(maxLength, chunk, hashedPassword);
            if (tmp != null && !tmp.isBlank()) {
                return tmp;
            }
        }
        throw new Exception();
    }
}
